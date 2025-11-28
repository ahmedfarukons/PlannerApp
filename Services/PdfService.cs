using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;

namespace StudyPlanner.Services
{
    /// <summary>
    /// PDF işleme servisi implementasyonu
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly IAiService _aiService;

        public PdfService(IAiService aiService)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        }

        public async Task<DocumentSummary> ProcessPdfAsync(string pdfPath)
        {
            if (!File.Exists(pdfPath))
                throw new FileNotFoundException("PDF dosyası bulunamadı", pdfPath);

            // PDF bilgilerini al
            var fileInfo = new FileInfo(pdfPath);
            var text = await ExtractTextAsync(pdfPath);

            // AI ile özet ve model çıkarma
            var summaryTask = _aiService.GenerateSummaryAsync(text);
            var modelsTask = _aiService.ExtractModelsAsync(text);

            await Task.WhenAll(summaryTask, modelsTask);

            var summary = new DocumentSummary
            {
                FileName = Path.GetFileName(pdfPath),
                Summary = summaryTask.Result,
                ModelsUsed = modelsTask.Result,
                UploadDate = DateTime.Now,
                FileSize = fileInfo.Length,
                PageCount = GetPageCount(pdfPath)
            };

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

