using System.Threading.Tasks;

namespace StudyPlanner.Interfaces
{
    /// <summary>
    /// AI servisi arayüzü (Google Gemini)
    /// </summary>
    public interface IAiService
    {
        /// <summary>
        /// Metin için özet oluşturur
        /// </summary>
        /// <param name="text">Özetlenecek metin</param>
        /// <returns>Özet</returns>
        Task<string> GenerateSummaryAsync(string text);

        /// <summary>
        /// Metinden kullanılan modelleri/algoritmaları çıkarır
        /// </summary>
        /// <param name="text">Analiz edilecek metin</param>
        /// <returns>Bulunan modeller</returns>
        Task<string> ExtractModelsAsync(string text);

        /// <summary>
        /// Soru sorar ve cevap alır
        /// </summary>
        /// <param name="question">Soru</param>
        /// <param name="context">Bağlam (PDF içeriği)</param>
        /// <returns>Cevap</returns>
        Task<string> AskQuestionAsync(string question, string context);
    }
}

