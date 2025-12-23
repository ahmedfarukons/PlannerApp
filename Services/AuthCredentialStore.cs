using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace StudyPlanner.Services
{
    /// <summary>
    /// "Beni hatırla" için Windows DPAPI ile şifrelenmiş credential saklama.
    /// Not: Bu mekanizma sadece aynı Windows kullanıcı profili altında çözülür.
    /// </summary>
    public class AuthCredentialStore
    {
        private readonly string _filePath;

        public AuthCredentialStore()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, "StudyPlanner");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "auth.json");
        }

        public void Save(string identifier, string password)
        {
            var payload = new AuthPayload
            {
                Identifier = identifier ?? string.Empty,
                EncryptedPassword = Protect(password ?? string.Empty),
                SavedAtUtc = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json, Encoding.UTF8);
        }

        public bool TryLoad(out string identifier, out string password)
        {
            identifier = string.Empty;
            password = string.Empty;

            try
            {
                if (!File.Exists(_filePath))
                    return false;

                var json = File.ReadAllText(_filePath, Encoding.UTF8);
                var payload = JsonSerializer.Deserialize<AuthPayload>(json);
                if (payload == null || string.IsNullOrWhiteSpace(payload.Identifier) || string.IsNullOrWhiteSpace(payload.EncryptedPassword))
                    return false;

                identifier = payload.Identifier;
                password = Unprotect(payload.EncryptedPassword);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Clear()
        {
            try
            {
                if (File.Exists(_filePath))
                    File.Delete(_filePath);
            }
            catch
            {
                // ignore
            }
        }

        private static string Protect(string plaintext)
        {
            var bytes = Encoding.UTF8.GetBytes(plaintext);
            var protectedBytes = ProtectedData.Protect(bytes, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        private static string Unprotect(string protectedBase64)
        {
            var protectedBytes = Convert.FromBase64String(protectedBase64);
            var bytes = ProtectedData.Unprotect(protectedBytes, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }

        private sealed class AuthPayload
        {
            public string Identifier { get; set; } = string.Empty;
            public string EncryptedPassword { get; set; } = string.Empty;
            public DateTime SavedAtUtc { get; set; }
        }
    }
}


