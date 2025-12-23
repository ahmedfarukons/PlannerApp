using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using StudyPlanner.Interfaces;
using PdfDoc = StudyPlanner.Models.DocumentSummary;

namespace StudyPlanner.Services
{
    /// <summary>
    /// PDF işleme servisi implementasyonu
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly IAiService _aiService;
        private readonly IUserContext _userContext;
        private readonly IPdfDocumentRepository _pdfRepository;

        public PdfService(IAiService aiService, IUserContext userContext, IPdfDocumentRepository pdfRepository)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _pdfRepository = pdfRepository ?? throw new ArgumentNullException(nameof(pdfRepository));
        }

        public async Task<PdfDoc> ProcessPdfAsync(string pdfPath)
        {
            if (!File.Exists(pdfPath))
                throw new FileNotFoundException("PDF dosyası bulunamadı", pdfPath);

            // PDF bilgilerini al
            var fileInfo = new FileInfo(pdfPath);
            var text = await ExtractTextAsync(pdfPath);

            // AI ile özet ve model çıkarma
            string summaryResult = "Özet oluşturuluyor...";
            string modelsResult = "Modeller çıkarılıyor...";

            try 
            {
                summaryResult = await _aiService.GenerateSummaryAsync(text);
            }
            catch (Exception ex)
            {
                summaryResult = $"⚠️ AI Özeti Oluşturulamadı.\n\nHata: {ex.Message}\n\nNot: PDF içeriği analiz için hazır, soru sorabilirsiniz.";
            }

            try
            {
                modelsResult = await _aiService.ExtractModelsAsync(text);
            }
            catch (Exception ex)
            {
                modelsResult = $"⚠️ Modeller çıkarılamadı: {ex.Message}";
            }

            // await Task.WhenAll(summaryTask, modelsTask); // Artık gerek yok, sıralı ve güvenli çalışıyor

            var summary = new PdfDoc
            {
                FileName = Path.GetFileName(pdfPath),
                Summary = summaryResult,
                ModelsUsed = modelsResult,
                UploadDate = DateTime.Now,
                FileSize = fileInfo.Length,
                PageCount = GetPageCount(pdfPath)
            };

            // MongoDB'ye kaydet (kullanıcı giriş yaptıysa)
            if (_userContext.IsAuthenticated && !string.IsNullOrWhiteSpace(_userContext.UserId))
            {
                try
                {
                    var docId = await _pdfRepository.UpsertFromSummaryAsync(_userContext.UserId!, pdfPath, summary);
                    summary.DocumentId = docId;
                }
                catch
                {
                    // Persist hatası UI'yı bozmasın (özet yine gösterilsin)
                }
            }

            return summary;
        }

        public async Task<string> ExtractTextAsync(string pdfPath)
        {
            return await Task.Run(() =>
            {
                var sb = new StringBuilder();

                using (var pdfReader = new PdfReader(pdfPath))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {
                        var page = pdfDocument.GetPage(i);
                        var strategy = new SimpleTextExtractionStrategy();
                        var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                        sb.AppendLine(pageText);
                    }
                }

                return sb.ToString();
            });
        }

        private int GetPageCount(string pdfPath)
        {
            using (var pdfReader = new PdfReader(pdfPath))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                return pdfDocument.GetNumberOfPages();
            }
        }
    }
}

