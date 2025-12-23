using System.Threading.Tasks;

namespace StudyPlanner.Interfaces
{
    /// <summary>
    /// Dialog servis interface - Kullanıcı etkileşimi için
    /// SOLID: Interface Segregation Principle
    /// Testability için önemli - Mock edilebilir
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Bilgilendirme mesajı gösterir
        /// </summary>
        /// <param name="message">Mesaj içeriği</param>
        /// <param name="title">Mesaj başlığı</param>
        void ShowMessage(string message, string title = "Bilgi");

        /// <summary>
        /// Hata mesajı gösterir
        /// </summary>
        /// <param name="message">Hata mesajı</param>
        /// <param name="title">Mesaj başlığı</param>
        void ShowError(string message, string title = "Hata");

        /// <summary>
        /// Onay dialogu gösterir
        /// </summary>
        /// <param name="message">Onay mesajı</param>
        /// <param name="title">Dialog başlığı</param>
        /// <returns>Kullanıcı onayı (true/false)</returns>
        bool ShowConfirmation(string message, string title = "Onay");

        /// <summary>
        /// Dosya açma dialogu gösterir
        /// </summary>
        /// <param name="filter">Dosya filtresi</param>
        /// <returns>Seçilen dosya yolu veya null</returns>
        string? ShowOpenFileDialog(string filter = "XML Dosyaları (*.xml)|*.xml|Tüm Dosyalar (*.*)|*.*");

        /// <summary>
        /// Dosya kaydetme dialogu gösterir
        /// </summary>
        /// <param name="filter">Dosya filtresi</param>
        /// <param name="defaultFileName">Varsayılan dosya adı</param>
        /// <returns>Seçilen dosya yolu veya null</returns>
        string? ShowSaveFileDialog(string filter = "XML Dosyaları (*.xml)|*.xml|Tüm Dosyalar (*.*)|*.*", 
                                   string defaultFileName = "plans.xml");
    }
}



