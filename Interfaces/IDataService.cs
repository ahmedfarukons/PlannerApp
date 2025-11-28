using System.Threading.Tasks;

namespace StudyPlanner.Interfaces
{
    /// <summary>
    /// Veri servis interface - Dosya işlemleri için
    /// SOLID: Interface Segregation Principle
    /// </summary>
    /// <typeparam name="T">Veri tipi</typeparam>
    public interface IDataService<T> where T : class
    {
        /// <summary>
        /// Dosyadan veri yükler
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        /// <returns>Yüklenen veri</returns>
        Task<T> LoadFromFileAsync(string filePath);

        /// <summary>
        /// Dosyaya veri kaydeder
        /// </summary>
        /// <param name="data">Kaydedilecek veri</param>
        /// <param name="filePath">Dosya yolu</param>
        /// <returns>Kaydetme işlemi başarılı mı</returns>
        Task<bool> SaveToFileAsync(T data, string filePath);

        /// <summary>
        /// Varsayılan dosyadan veri yükler
        /// </summary>
        /// <returns>Yüklenen veri</returns>
        Task<T> LoadAsync();

        /// <summary>
        /// Varsayılan dosyaya veri kaydeder
        /// </summary>
        /// <param name="data">Kaydedilecek veri</param>
        /// <returns>Kaydetme işlemi başarılı mı</returns>
        Task<bool> SaveAsync(T data);
    }
}



