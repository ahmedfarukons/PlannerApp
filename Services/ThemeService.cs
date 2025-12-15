using System;
using System.IO;
using System.Windows;

namespace StudyPlanner.Services
{
    /// <summary>
    /// Tema yönetim servisi
    /// Light/Dark mode geçişi
    /// </summary>
    public class ThemeService
    {
        private const string ThemeSettingFile = "theme_setting.txt";
        private readonly string _themeFilePath;

        public ThemeService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "StudyPlanner");
            
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            _themeFilePath = Path.Combine(appFolder, ThemeSettingFile);
        }

        /// <summary>
        /// Mevcut tema
        /// </summary>
        public string CurrentTheme { get; private set; } = "Light";

        /// <summary>
        /// Dark mode aktif mi?
        /// </summary>
        public bool IsDarkMode => CurrentTheme == "Dark";

        /// <summary>
        /// Temayı değiştir
        /// </summary>
        public void ToggleTheme()
        {
            if (IsDarkMode)
                ApplyTheme("Light");
            else
                ApplyTheme("Dark");
        }

        /// <summary>
        /// Belirli bir tema uygula
        /// </summary>
        public void ApplyTheme(string themeName)
        {
            try
            {
                CurrentTheme = themeName;

                // Resource dictionary'yi temizle ve yenisini ekle
                var app = Application.Current;
                
                // Eski tema resource'unu bul ve çıkar
                ResourceDictionary? oldTheme = null;
                foreach (var dict in app.Resources.MergedDictionaries)
                {
                    if (dict.Source != null && dict.Source.OriginalString.Contains("Themes/"))
                    {
                        oldTheme = dict;
                        break;
                    }
                }

                if (oldTheme != null)
                    app.Resources.MergedDictionaries.Remove(oldTheme);

                // Yeni temayı ekle
                var themeUri = new Uri($"Themes/{themeName}Theme.xaml", UriKind.Relative);
                var newTheme = new ResourceDictionary { Source = themeUri };
                app.Resources.MergedDictionaries.Add(newTheme);

                // Ayarı kaydet
                SaveThemeSetting(themeName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tema değiştirme hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Kaydedilmiş temayı yükle
        /// </summary>
        public void LoadSavedTheme()
        {
            try
            {
                if (File.Exists(_themeFilePath))
                {
                    var savedTheme = File.ReadAllText(_themeFilePath).Trim();
                    if (savedTheme == "Light" || savedTheme == "Dark")
                    {
                        ApplyTheme(savedTheme);
                        return;
                    }
                }
            }
            catch
            {
                // Hata durumunda default tema
            }

            // Default tema
            ApplyTheme("Light");
        }

        /// <summary>
        /// Tema ayarını kaydet
        /// </summary>
        private void SaveThemeSetting(string themeName)
        {
            try
            {
                File.WriteAllText(_themeFilePath, themeName);
            }
            catch
            {
                // Kaydetme hatası - sessizce devam et
            }
        }
    }
}


