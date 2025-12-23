using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using StudyPlanner.Interfaces;

namespace StudyPlanner.Services
{
    /// <summary>
    /// Google Gemini AI servisi implementasyonu
    /// </summary>
    public class SGeminiAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiBaseUrl;
        private readonly double _temperature;
        private readonly int _maxOutputTokens;
        private readonly string _apiKeySource;
        private readonly string[] _modelCandidates;
        private readonly object _modelResolutionLock = new object();
        private string? _resolvedModelBaseUrl; // cached working model base url (no key query string)

        // Not: Google endpointleri zaman içinde sürüm değiştiriyor. v1beta bazı anahtarlar/modeller için 404 döndürebiliyor.
        private const string DefaultApiBaseUrlV1 =
            "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent";

        private const string DefaultApiBaseUrlV1beta =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        // En yaygın çalışan fallback sırası (model erişimi anahtara göre değişebilir)
        private static readonly string[] DefaultModelCandidates = new[]
        {
            "gemini-1.5-flash-latest",
            "gemini-1.5-flash",
            "gemini-1.5-pro-latest",
            "gemini-1.5-pro",
            "gemini-pro"
        };

        public SGeminiAiService(
            string apiKey,
            string apiBaseUrl,
            double temperature = 0.1,
            int maxOutputTokens = 2048,
            HttpClient? httpClient = null,
            string? apiKeySource = null,
            string? modelCandidatesCsv = null)
        {
            _apiKey = (apiKey ?? throw new ArgumentNullException(nameof(apiKey))).Trim();
            _apiBaseUrl = string.IsNullOrWhiteSpace(apiBaseUrl) ? DefaultApiBaseUrlV1 : apiBaseUrl.Trim();
            _temperature = temperature;
            _maxOutputTokens = maxOutputTokens;
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(90);
            _apiKeySource = string.IsNullOrWhiteSpace(apiKeySource) ? "Unknown" : apiKeySource.Trim();
            _modelCandidates = ParseModelCandidates(modelCandidatesCsv);
        }

        public async Task<string> GenerateSummaryAsync(string text)
        {
            try
            {
                var prompt = $@"
Bu akademik makalenin kısa bir özetini çıkar. Önemli bulguları, araştırma yöntemlerini ve ana argümanları vurgula.
Özet tek paragraf olmalı ve 3-4 cümleyi geçmemeli. Kullanılan modeller hakkında bilgi varsa bunu mutlaka içer.

Makale:
{text.Substring(0, Math.Min(7000, text.Length))}";

                return await CallGeminiApiAsync(prompt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Özet oluşturma hatası: {ex.Message}", ex);
            }
        }

        public async Task<string> ExtractModelsAsync(string text)
        {
            try
            {
                var prompt = $@"
Aşağıdaki akademik makale metninden, kullanılan algoritma, model ve teknik isimleri tespit et.
Sadece metinde geçen spesifik metotları listele. Tahmin yürütme.

Metin:
{text.Substring(0, Math.Min(10000, text.Length))}";

                return await CallGeminiApiAsync(prompt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Model çıkarma hatası: {ex.Message}", ex);
            }
        }

        public async Task<string> AskQuestionAsync(string question, string context)
        {
            try
            {
                var prompt = $@"
Sen bir akademik makale uzmanı AI asistansın. Makalenin içeriğine sadık kalarak soruları yanıtla.
Eğer makale içinde bir bilgi yoksa, bunu açıkça belirt ve tahmin yürütme.
Makalede kullanılan modeller, algoritmalar ve teknikler hakkında sorulursa, makaledeki bilgilere dayanarak detaylı cevap ver.

Makale İçeriği:
{context.Substring(0, Math.Min(15000, context.Length))}

Soru: {question}

Cevap:";

                return await CallGeminiApiAsync(prompt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Soru cevaplama hatası: {ex.Message}", ex);
            }
        }

        private async Task<string> CallGeminiApiAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new Exception("Google API Key boş. Lütfen API Key'inizi ayarlayın.");

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = _temperature,
                    maxOutputTokens = _maxOutputTokens
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            
            var maxAttempts = 3;
            var triedDefaultFallback = false;
            var triedVersionSwap = false;
            var triedModelFallback = false;

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                // Eğer daha önce çalışan bir model bulduysak direkt onu dene
                var baseUrlForThisAttempt = GetResolvedModelBaseUrlOrDefault();

                if (triedVersionSwap)
                {
                    var swapped = TrySwapApiVersion(baseUrlForThisAttempt);
                    if (!string.IsNullOrWhiteSpace(swapped))
                        baseUrlForThisAttempt = swapped;
                }

                if (triedDefaultFallback)
                {
                    // Fallback sırası: v1 -> v1beta
                    baseUrlForThisAttempt = triedVersionSwap ? DefaultApiBaseUrlV1beta : DefaultApiBaseUrlV1;
                }

                HttpResponseMessage? response = null;
                string? errorContent = null;

                try
                {
                    using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync(BuildUrlWithKey(baseUrlForThisAttempt, _apiKey), content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        CacheResolvedModelBaseUrl(baseUrlForThisAttempt);
                        return ExtractTextFromResponse(responseJson);
                    }

                    errorContent = await response.Content.ReadAsStringAsync();

                    // Yanlış model/baseUrl gibi 404 durumlarında, eğer konfigürasyon farklıysa varsayılan endpoint'e bir kez düş.
                    if (response.StatusCode == HttpStatusCode.NotFound &&
                        attempt < maxAttempts)
                    {
                        // 0) Önce model adı fallback dene (flash-latest, pro-latest, gemini-pro vs.)
                        if (!triedModelFallback && LooksLikeModelNotFound(errorContent))
                        {
                            triedModelFallback = true;
                            var maybeWorkingBaseUrl = await TryResolveWorkingModelBaseUrlAsync(baseUrlForThisAttempt, requestJson);
                            if (!string.IsNullOrWhiteSpace(maybeWorkingBaseUrl))
                            {
                                CacheResolvedModelBaseUrl(maybeWorkingBaseUrl);
                                // Cache'ledik; aynı attempt içinde bir tur daha denemek için continue
                                continue;
                            }
                        }

                        // 1) Önce v1 <-> v1beta versiyonunu swap etmeyi dene
                        if (!triedVersionSwap)
                        {
                            triedVersionSwap = true;
                            continue;
                        }

                        // 2) Son çare: varsayılan endpointlere düş (v1, sonra v1beta)
                        if (!triedDefaultFallback)
                        {
                            triedDefaultFallback = true;
                            triedVersionSwap = false;
                            continue;
                        }

                        // 3) İkinci varsayılan (v1beta) için bir tur daha
                        if (triedDefaultFallback && !triedVersionSwap)
                        {
                            triedVersionSwap = true;
                            continue;
                        }
                        continue;
                    }

                    // Transient hatalarda retry
                    if (IsTransientStatusCode(response.StatusCode) && attempt < maxAttempts)
                    {
                        var delay = GetRetryDelay(response, attempt);
                        await Task.Delay(delay);
                        continue;
                    }

                    throw new Exception(BuildApiErrorMessage(response.StatusCode, baseUrlForThisAttempt, errorContent));
                }
                catch (TaskCanceledException) when (attempt < maxAttempts)
                {
                    // Timeout veya iptal. Timeout ise tekrar dene.
                    await Task.Delay(GetBackoffDelay(attempt));
                    continue;
                }
                catch (HttpRequestException) when (attempt < maxAttempts)
                {
                    // Ağ / proxy / DNS / TLS gibi durumlar olabilir. Tekrar dene.
                    await Task.Delay(GetBackoffDelay(attempt));
                    continue;
                }
            }

            // Should never hit (loop either returns or throws)
            throw new Exception("API çağrısı başarısız oldu.");
        }

        private string GetResolvedModelBaseUrlOrDefault()
        {
            lock (_modelResolutionLock)
            {
                return string.IsNullOrWhiteSpace(_resolvedModelBaseUrl) ? _apiBaseUrl : _resolvedModelBaseUrl!;
            }
        }

        private void CacheResolvedModelBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return;

            lock (_modelResolutionLock)
            {
                _resolvedModelBaseUrl = baseUrl.Trim();
            }
        }

        private static string[] ParseModelCandidates(string? csv)
        {
            if (string.IsNullOrWhiteSpace(csv))
                return DefaultModelCandidates;

            var parts = csv.Split(new[] { ',', ';', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var list = new System.Collections.Generic.List<string>();
            foreach (var p in parts)
            {
                var v = p.Trim();
                if (v.Length == 0)
                    continue;
                // config'te yanlışlıkla models/ ile girilirse normalize et
                if (v.StartsWith("models/", StringComparison.OrdinalIgnoreCase))
                    v = v.Substring("models/".Length);
                list.Add(v);
            }
            return list.Count == 0 ? DefaultModelCandidates : list.ToArray();
        }

        private static bool LooksLikeModelNotFound(string? errorContent)
        {
            if (string.IsNullOrWhiteSpace(errorContent))
                return false;

            // Google error JSON'unda genelde: "models/<name> is not found ..." veya status: NOT_FOUND
            return errorContent.Contains("is not found", StringComparison.OrdinalIgnoreCase) ||
                   errorContent.Contains("\"status\":\"NOT_FOUND\"", StringComparison.OrdinalIgnoreCase) ||
                   errorContent.Contains("\"status\": \"NOT_FOUND\"", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<string?> TryResolveWorkingModelBaseUrlAsync(string baseUrl, string requestJson)
        {
            // 1) Önce aday model isimleriyle dene (mevcut api versiyonunu koru)
            foreach (var candidate in _modelCandidates)
            {
                var candidateBaseUrl = ReplaceModelInBaseUrl(baseUrl, candidate);
                if (string.IsNullOrWhiteSpace(candidateBaseUrl))
                    continue;

                var ok = await TryPostSuccessfulAsync(candidateBaseUrl, requestJson);
                if (ok)
                    return candidateBaseUrl;
            }

            // 2) Son çare: ListModels ile anahtarın erişebildiği modellerden uygun olanı seç
            var listedModel = await TryPickModelFromListModelsAsync();
            if (!string.IsNullOrWhiteSpace(listedModel))
            {
                var pickedBaseUrl = ReplaceModelInBaseUrl(baseUrl, listedModel!);
                if (!string.IsNullOrWhiteSpace(pickedBaseUrl))
                {
                    var ok = await TryPostSuccessfulAsync(pickedBaseUrl, requestJson);
                    if (ok)
                        return pickedBaseUrl;
                }
            }

            return null;
        }

        private async Task<bool> TryPostSuccessfulAsync(string baseUrl, string requestJson)
        {
            try
            {
                using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                using var resp = await _httpClient.PostAsync(BuildUrlWithKey(baseUrl, _apiKey), content);
                return resp.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string?> TryPickModelFromListModelsAsync()
        {
            try
            {
                var url = BuildUrlWithKey("https://generativelanguage.googleapis.com/v1/models", _apiKey);
                using var resp = await _httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                    return null;

                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("models", out var models) ||
                    models.ValueKind != JsonValueKind.Array)
                    return null;

                // Preference: bizim aday listemizde olan ve generateContent destekleyen ilk modeli seç
                foreach (var wanted in _modelCandidates)
                {
                    var found = FindModelInList(models, wanted);
                    if (!string.IsNullOrWhiteSpace(found))
                        return found;
                }

                // Fallback: generateContent destekleyen ilk modeli dön
                foreach (var model in models.EnumerateArray())
                {
                    if (!model.TryGetProperty("name", out var nameProp))
                        continue;
                    var name = nameProp.GetString();
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    if (ModelSupportsGenerateContent(model))
                        return NormalizeModelName(name);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string? FindModelInList(JsonElement modelsArray, string wantedModel)
        {
            foreach (var model in modelsArray.EnumerateArray())
            {
                if (!model.TryGetProperty("name", out var nameProp))
                    continue;
                var name = nameProp.GetString();
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var normalized = NormalizeModelName(name);
                if (!string.Equals(normalized, wantedModel, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (ModelSupportsGenerateContent(model))
                    return normalized;
            }
            return null;
        }

        private static bool ModelSupportsGenerateContent(JsonElement model)
        {
            if (!model.TryGetProperty("supportedGenerationMethods", out var methods) ||
                methods.ValueKind != JsonValueKind.Array)
                return true; // eski response'larda alan olmayabilir, engel olmayalım

            foreach (var m in methods.EnumerateArray())
            {
                var s = m.GetString();
                if (string.Equals(s, "generateContent", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static string NormalizeModelName(string apiModelName)
        {
            // API genelde "models/gemini-..." döndürür
            if (apiModelName.StartsWith("models/", StringComparison.OrdinalIgnoreCase))
                return apiModelName.Substring("models/".Length);
            return apiModelName;
        }

        private static string? ReplaceModelInBaseUrl(string baseUrl, string newModel)
        {
            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(newModel))
                return null;

            // https://.../(v1|v1beta)/models/{model}:generateContent
            // model segmentini yakalayıp değiştir
            var normalizedModel = newModel.Trim();
            if (normalizedModel.StartsWith("models/", StringComparison.OrdinalIgnoreCase))
                normalizedModel = normalizedModel.Substring("models/".Length);

            var pattern = @"(/models/)([^:/]+)(:generateContent)";
            if (!Regex.IsMatch(baseUrl, pattern, RegexOptions.IgnoreCase))
                return null;

            return Regex.Replace(baseUrl, pattern, $"$1{normalizedModel}$3", RegexOptions.IgnoreCase);
        }

        private static string BuildUrlWithKey(string baseUrl, string apiKey)
        {
            // baseUrl already has query? append with & otherwise ?
            if (baseUrl.Contains("key=", StringComparison.OrdinalIgnoreCase))
                return baseUrl;

            var separator = baseUrl.Contains("?", StringComparison.Ordinal) ? "&" : "?";
            return $"{baseUrl}{separator}key={Uri.EscapeDataString(apiKey)}";
        }

        private static bool IsTransientStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.TooManyRequests ||
                   statusCode == HttpStatusCode.InternalServerError ||
                   statusCode == HttpStatusCode.BadGateway ||
                   statusCode == HttpStatusCode.ServiceUnavailable ||
                   statusCode == HttpStatusCode.GatewayTimeout;
        }

        private static TimeSpan GetBackoffDelay(int attempt)
        {
            // 1 -> 800ms, 2 -> 1600ms, 3 -> 2400ms (cap)
            var ms = Math.Min(2400, 800 * attempt);
            return TimeSpan.FromMilliseconds(ms);
        }

        private static TimeSpan GetRetryDelay(HttpResponseMessage response, int attempt)
        {
            if (response.Headers.RetryAfter?.Delta is TimeSpan delta)
                return delta;

            if (response.Headers.RetryAfter?.Date is DateTimeOffset date)
            {
                var diff = date - DateTimeOffset.UtcNow;
                if (diff > TimeSpan.Zero)
                    return diff;
            }

            return GetBackoffDelay(attempt);
        }

        private string BuildApiErrorMessage(HttpStatusCode statusCode, string baseUrl, string? errorContent)
        {
            var sb = new StringBuilder();
            sb.Append($"API hatası ({(int)statusCode} {statusCode}). ");

            // Google error JSON'u parse etmeye çalış
            var parsed = TryParseGoogleError(errorContent, out var googleStatus, out var googleMessage);
            if (parsed)
            {
                sb.Append($"Google Error: {googleStatus} - {googleMessage}. ");
            }
            else if (!string.IsNullOrWhiteSpace(errorContent))
            {
                sb.Append("Detay: ");
                sb.Append(TrimForUi(errorContent, 1200));
                sb.Append(". ");
            }

            // Actionable hint'ler
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    sb.Append("Muhtemel neden: API key geçersiz. ");
                    break;
                case HttpStatusCode.Forbidden:
                    sb.Append("Muhtemel neden: API key kısıtlı/engelli veya API etkin değil. ");
                    break;
                case HttpStatusCode.NotFound:
                    sb.Append("Muhtemel neden: model adı/baseUrl yanlış (v1beta endpoint + model uyumu). ");
                    break;
                case HttpStatusCode.TooManyRequests:
                    sb.Append("Muhtemel neden: rate-limit/kota. Biraz bekleyip tekrar deneyin. ");
                    break;
                case HttpStatusCode.BadRequest:
                    sb.Append("Muhtemel neden: istek formatı veya model parametreleri geçersiz. ");
                    break;
            }

            sb.Append($"(ApiBaseUrl: {baseUrl}, ApiKeySource: {_apiKeySource})");
            return sb.ToString();
        }

        private static bool TryParseGoogleError(string? errorContent, out string status, out string message)
        {
            status = "";
            message = "";

            if (string.IsNullOrWhiteSpace(errorContent))
                return false;

            try
            {
                using var doc = JsonDocument.Parse(errorContent);
                if (!doc.RootElement.TryGetProperty("error", out var error))
                    return false;

                if (error.TryGetProperty("status", out var statusProp))
                    status = statusProp.GetString() ?? "";

                if (error.TryGetProperty("message", out var msgProp))
                    message = msgProp.GetString() ?? "";

                return !string.IsNullOrWhiteSpace(status) || !string.IsNullOrWhiteSpace(message);
            }
            catch
            {
                return false;
            }
        }

        private static string TrimForUi(string value, int maxChars)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxChars)
                return value;

            return value.Substring(0, maxChars) + "…";
        }

        private static string ExtractTextFromResponse(string responseJson)
        {
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                candidates.ValueKind == JsonValueKind.Array &&
                candidates.GetArrayLength() > 0)
            {
                var firstCandidate = candidates[0];
                if (firstCandidate.TryGetProperty("content", out var contentElement) &&
                    contentElement.TryGetProperty("parts", out var parts) &&
                    parts.ValueKind == JsonValueKind.Array &&
                    parts.GetArrayLength() > 0)
                {
                    var firstPart = parts[0];
                    if (firstPart.TryGetProperty("text", out var textElement))
                    {
                        var result = textElement.GetString();
                        if (!string.IsNullOrWhiteSpace(result))
                            return result;
                    }
                }
            }

            throw new Exception("API'dan geçersiz yanıt alındı (candidates/content/parts/text bulunamadı).");
        }

        private static string? TrySwapApiVersion(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return null;

            if (baseUrl.Contains("/v1beta/", StringComparison.OrdinalIgnoreCase))
                return baseUrl.Replace("/v1beta/", "/v1/", StringComparison.OrdinalIgnoreCase);

            if (baseUrl.Contains("/v1/", StringComparison.OrdinalIgnoreCase))
                return baseUrl.Replace("/v1/", "/v1beta/", StringComparison.OrdinalIgnoreCase);

            return null;
        }
    }
}

