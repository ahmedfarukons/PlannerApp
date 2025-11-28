using System.Threading.Tasks;
using StudyPlanner.Models;

namespace StudyPlanner.Interfaces
{
    /// <summary>
    /// PDF işleme servisi arayüzü
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// PDF dosyasını işler ve özet çıkarır
        /// </summary>
        /// <param name="pdfPath">PDF dosya yolu</param>
        /// <returns>Döküman özeti</returns>
        Task<DocumentSummary> ProcessPdfAsync(string pdfPath);

        /// <summary>
        /// PDF'den metin çıkarır
        /// </summary>
        /// <param name="pdfPath">PDF dosya yolu</param>
        /// <returns>Çıkarılan metin</returns>
        Task<string> ExtractTextAsync(string pdfPath);
    }
}

