using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace StudyPlanner.Helpers
{
    /// <summary>
    /// Konfigürasyon yardımcı sınıfı
    /// appsettings.json ve .env dosyalarını okur
    /// </summary>
    public static class ConfigurationHelper
    {
        private static IConfiguration? _configuration;
        private static readonly object _lock = new object();
        private const string GoogleApiKeyEnvVarName = "GOOGLE_API_KEY";

        /// <summary>
        /// Konfigürasyonu yükler
        /// </summary>
        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    lock (_lock)
                    {
                        if (_configuration == null)
                        {
                            LoadConfiguration();
                        }
                    }
                }
                return _configuration!;
            }
        }

        /// <summary>
        /// Konfigürasyonu yükler (appsettings.json + .env)
        /// </summary>
        private static void LoadConfiguration()
        {
            try
            {
                // .env dosyasını önce yükle
                LoadEnvFile();

                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddJsonFile($"appsettings.{GetEnvironment()}.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables();

                _configuration = builder.Build();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Konfigürasyon yüklenirken hata oluştu!", ex);
            }
        }

        /// <summary>
        /// .env dosyasını okur ve environment variable'lara yükler
        /// </summary>
        private static void LoadEnvFile()
        {
            // Bazı çalıştırma senaryolarında CurrentDirectory ve BaseDirectory farklı olabilir.
            // İkisini de deneriz (örn: Visual Studio, dotnet run, yayınlanan exe).
            var candidateEnvFiles = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env")
            };

            string? envFile = null;
            foreach (var candidate in candidateEnvFiles)
            {
                if (File.Exists(candidate))
                {
                    envFile = candidate;
                    break;
                }
            }

            if (envFile == null)
                return;

            foreach (var line in File.ReadAllLines(envFile))
            {
                // Boş satır veya yorum satırını atla
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        /// <summary>
        /// Environment (Development, Production) döndürür
        /// </summary>
        private static string GetEnvironment()
        {
            return Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                ?? "Development";
        }

        /// <summary>
        /// Google API Key'i döndürür
        /// Önce .env, sonra appsettings.json, sonra environment variable'dan okur
        /// </summary>
        public static string GetGoogleApiKey()
        {
            if (TryGetGoogleApiKey(out var key, out _))
                return key;

            throw new InvalidOperationException(
                "Google API Key bulunamadı!\n\n" +
                "Lütfen aşağıdaki yöntemlerden birini kullanın:\n" +
                "1. .env dosyasına GOOGLE_API_KEY=your_key_here ekleyin\n" +
                "2. appsettings.Development.json içinde ApiSettings:GoogleApiKey ayarlayın\n" +
                "3. Environment variable olarak GOOGLE_API_KEY tanımlayın\n\n" +
                "API Key almak için: https://makersuite.google.com/app/apikey"
            );
        }

        /// <summary>
        /// Google API Key'i ve kaynağını (env/appsettings) döndürür.
        /// </summary>
        public static bool TryGetGoogleApiKey(out string apiKey, out string source)
        {
            // 1. Environment variable'dan (en yüksek öncelik)
            var envKey = Environment.GetEnvironmentVariable(GoogleApiKeyEnvVarName);
            if (!string.IsNullOrWhiteSpace(envKey))
            {
                apiKey = envKey.Trim();
                source = $"Environment Variable ({GoogleApiKeyEnvVarName})";
                return true;
            }

            // 2. appsettings.*.json'dan
            var configKey = Configuration["ApiSettings:GoogleApiKey"];
            if (!string.IsNullOrWhiteSpace(configKey))
            {
                apiKey = configKey.Trim();
                source = $"appsettings.{GetEnvironment()}.json (ApiSettings:GoogleApiKey)";
                return true;
            }

            apiKey = string.Empty;
            source = "Not Found";
            return false;
        }

        /// <summary>
        /// Teşhis için API key kaynağını döndürür (key'i asla döndürmez).
        /// </summary>
        public static string GetGoogleApiKeySource()
        {
            return TryGetGoogleApiKey(out _, out var source) ? source : "Not Found";
        }

        /// <summary>
        /// Konfigürasyondan değer okur
        /// </summary>
        public static string GetValue(string key, string defaultValue = "")
        {
            return Configuration[key] ?? defaultValue;
        }

        /// <summary>
        /// Konfigürasyondan int değer okur
        /// </summary>
        public static int GetIntValue(string key, int defaultValue = 0)
        {
            var value = Configuration[key];
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// Konfigürasyondan double değer okur
        /// </summary>
        public static double GetDoubleValue(string key, double defaultValue = 0.0)
        {
            var value = Configuration[key];
            return double.TryParse(value, out var result) ? result : defaultValue;
        }
    }
}

