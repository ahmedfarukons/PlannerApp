using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StudyPlanner.Interfaces;

namespace StudyPlanner.Services
{
    /// <summary>
    /// Google Gemini AI servisi implementasyonu
    /// </summary>
    public class GeminiAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

        public GeminiAiService(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _httpClient = new HttpClient();
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
                    temperature = 0.1,
                    maxOutputTokens = 2048
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}?key={_apiKey}", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API hatası: {response.StatusCode} - {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(responseJson);
            
            if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                candidates.GetArrayLength() > 0)
            {
                var firstCandidate = candidates[0];
                if (firstCandidate.TryGetProperty("content", out var contentElement) &&
                    contentElement.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0)
                {
                    var firstPart = parts[0];
                    if (firstPart.TryGetProperty("text", out var textElement))
                    {
                        return textElement.GetString();
                    }
                }
            }

            throw new Exception("API'dan geçersiz yanıt alındı");
        }
    }
}

