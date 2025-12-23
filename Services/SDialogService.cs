using System.Windows;
using Microsoft.Win32;
using StudyPlanner.Interfaces;

namespace StudyPlanner.Services
{
    /// <summary>
    /// Dialog işlemleri için servis
    /// SOLID: Single Responsibility - Sadece kullanıcı etkileşimi sorumluluğu
    /// Testability için interface kullanır
    /// </summary>
    public class SDialogService : IDialogService
    {
        /// <summary>
        /// Bilgilendirme mesajı gösterir
        /// </summary>
        public void ShowMessage(string message, string title = "Bilgi")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Hata mesajı gösterir
        /// </summary>
        public void ShowError(string message, string title = "Hata")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Onay dialogu gösterir
        /// </summary>
        public bool ShowConfirmation(string message, string title = "Onay")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Dosya açma dialogu gösterir
        /// </summary>
        public string? ShowOpenFileDialog(string filter = "XML Dosyaları (*.xml)|*.xml|Tüm Dosyalar (*.*)|*.*")
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter,
                Title = "Dosya Aç",
                CheckFileExists = true,
                CheckPathExists = true
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        /// <summary>
        /// Dosya kaydetme dialogu gösterir
        /// </summary>
        public string? ShowSaveFileDialog(string filter = "XML Dosyaları (*.xml)|*.xml|Tüm Dosyalar (*.*)|*.*", 
                                         string defaultFileName = "plans.xml")
        {
            var dialog = new SaveFileDialog
            {
                Filter = filter,
                Title = "Dosyayı Kaydet",
                FileName = defaultFileName,
                DefaultExt = ".xml",
                AddExtension = true
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}



