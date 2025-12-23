using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using StudyPlanner.Models;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Services
{
    /// <summary>
    /// PDF export servisi
    /// Çalışma planları ve istatistikleri PDF'e aktarır
    /// </summary>
    public class SPdfExportService
    {
        /// <summary>
        /// Çalışma planlarını PDF olarak export et
        /// </summary>
        public async Task ExportStudyPlansAsync(List<StudyPlanItem> plans, string outputPath)
        {
            await Task.Run(() =>
            {
                using (var writer = new PdfWriter(outputPath))
                using (var pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                {
                    var document = new Document(pdf);

                    // Başlık
                    AddTitle(document, "Çalışma Planları Raporu");
                    AddSubtitle(document, $"Oluşturma Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    document.Add(new Paragraph("\n"));

                    // Özet bilgiler
                    AddSummarySection(document, plans);

                    // Planlar tablosu
                    AddPlansTable(document, plans);

                    document.Close();
                }
            });
        }

        /// <summary>
        /// İstatistikleri PDF olarak export et
        /// </summary>
        public async Task ExportStatisticsAsync(
            int totalPlans,
            int completedPlans,
            double completionRate,
            double totalHours,
            List<CategoryStatistic> categoryStats,
            List<StudyStatistic> weeklyStats,
            string outputPath)
        {
            await Task.Run(() =>
            {
                using (var writer = new PdfWriter(outputPath))
                using (var pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                {
                    var document = new Document(pdf);

                    // Başlık
                    AddTitle(document, "Çalışma İstatistikleri Raporu");
                    AddSubtitle(document, $"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    document.Add(new Paragraph("\n"));

                    // Genel istatistikler
                    AddGeneralStats(document, totalPlans, completedPlans, completionRate, totalHours);

                    // Haftalık istatistikler
                    AddWeeklyStatsTable(document, weeklyStats);

                    // Kategori istatistikleri
                    AddCategoryStatsTable(document, categoryStats);

                    document.Close();
                }
            });
        }

        #region Helper Methods

        private void AddTitle(Document document, string title)
        {
            var titleParagraph = new Paragraph(title)
                .SetFontSize(24)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(33, 150, 243));
            
            document.Add(titleParagraph);
        }

        private void AddSubtitle(Document document, string subtitle)
        {
            var subtitleParagraph = new Paragraph(subtitle)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(ColorConstants.GRAY);
            
            document.Add(subtitleParagraph);
        }

        private void AddSummarySection(Document document, List<StudyPlanItem> plans)
        {
            var completed = plans.Count(p => p.IsCompleted);
            var total = plans.Count;
            var totalMinutes = plans.Where(p => p.IsCompleted).Sum(p => p.DurationMinutes);
            var completionRate = total > 0 ? (double)completed / total * 100 : 0;

            document.Add(new Paragraph("ÖZET BİLGİLER")
                .SetFontSize(16)
                .SetBold()
                .SetMarginTop(10));

            var summaryTable = new Table(new float[] { 1, 1, 1, 1 })
                .UseAllAvailableWidth()
                .SetMarginTop(10);

            // Header
            summaryTable.AddHeaderCell(CreateCell("Toplam Plan", true));
            summaryTable.AddHeaderCell(CreateCell("Tamamlanan", true));
            summaryTable.AddHeaderCell(CreateCell("Başarı Oranı", true));
            summaryTable.AddHeaderCell(CreateCell("Toplam Saat", true));

            // Data
            summaryTable.AddCell(CreateCell(total.ToString()));
            summaryTable.AddCell(CreateCell(completed.ToString()));
            summaryTable.AddCell(CreateCell($"%{completionRate:F1}"));
            summaryTable.AddCell(CreateCell($"{totalMinutes / 60.0:F1}h"));

            document.Add(summaryTable);
            document.Add(new Paragraph("\n"));
        }

        private void AddPlansTable(Document document, List<StudyPlanItem> plans)
        {
            document.Add(new Paragraph("ÇALIŞMA PLANLARI")
                .SetFontSize(16)
                .SetBold()
                .SetMarginTop(10));

            var table = new Table(new float[] { 2, 1, 1, 1, 1 })
                .UseAllAvailableWidth()
                .SetMarginTop(10);

            // Headers
            table.AddHeaderCell(CreateCell("Ders/Konu", true));
            table.AddHeaderCell(CreateCell("Tarih", true));
            table.AddHeaderCell(CreateCell("Süre", true));
            table.AddHeaderCell(CreateCell("Öncelik", true));
            table.AddHeaderCell(CreateCell("Durum", true));

            // Data
            foreach (var plan in plans.OrderBy(p => p.Date))
            {
                table.AddCell(CreateCell(plan.Subject));
                table.AddCell(CreateCell(plan.Date.ToString("dd.MM.yyyy")));
                table.AddCell(CreateCell($"{plan.DurationMinutes}dk"));
                table.AddCell(CreateCell(GetPriorityText(plan.Priority)));
                table.AddCell(CreateCell(plan.IsCompleted ? "✓ Tamamlandı" : "⏳ Bekliyor"));
            }

            document.Add(table);
        }

        private void AddGeneralStats(Document document, int totalPlans, int completedPlans, 
            double completionRate, double totalHours)
        {
            document.Add(new Paragraph("GENEL İSTATİSTİKLER")
                .SetFontSize(16)
                .SetBold()
                .SetMarginTop(10));

            var statsTable = new Table(new float[] { 1, 1, 1, 1 })
                .UseAllAvailableWidth()
                .SetMarginTop(10);

            // Header
            statsTable.AddHeaderCell(CreateCell("Toplam Plan", true));
            statsTable.AddHeaderCell(CreateCell("Tamamlanan", true));
            statsTable.AddHeaderCell(CreateCell("Başarı Oranı", true));
            statsTable.AddHeaderCell(CreateCell("Toplam Çalışma", true));

            // Data
            statsTable.AddCell(CreateCell(totalPlans.ToString()));
            statsTable.AddCell(CreateCell(completedPlans.ToString()));
            statsTable.AddCell(CreateCell($"%{completionRate:F1}"));
            statsTable.AddCell(CreateCell($"{totalHours:F1} saat"));

            document.Add(statsTable);
            document.Add(new Paragraph("\n"));
        }

        private void AddWeeklyStatsTable(Document document, List<StudyStatistic> weeklyStats)
        {
            if (weeklyStats == null || !weeklyStats.Any())
                return;

            document.Add(new Paragraph("HAFTALIK İSTATİSTİKLER")
                .SetFontSize(16)
                .SetBold()
                .SetMarginTop(10));

            var table = new Table(new float[] { 1, 1, 1 })
                .UseAllAvailableWidth()
                .SetMarginTop(10);

            // Headers
            table.AddHeaderCell(CreateCell("Gün", true));
            table.AddHeaderCell(CreateCell("Tamamlanan Plan", true));
            table.AddHeaderCell(CreateCell("Çalışma Saati", true));

            // Data
            foreach (var stat in weeklyStats)
            {
                table.AddCell(CreateCell($"{stat.DayName} ({stat.Date:dd.MM})"));
                table.AddCell(CreateCell(stat.CompletedCount.ToString()));
                table.AddCell(CreateCell($"{stat.Hours:F1}h"));
            }

            document.Add(table);
            document.Add(new Paragraph("\n"));
        }

        private void AddCategoryStatsTable(Document document, List<CategoryStatistic> categoryStats)
        {
            if (categoryStats == null || !categoryStats.Any())
                return;

            document.Add(new Paragraph("KATEGORİ İSTATİSTİKLERİ")
                .SetFontSize(16)
                .SetBold()
                .SetMarginTop(10));

            var table = new Table(new float[] { 2, 1, 1, 1 })
                .UseAllAvailableWidth()
                .SetMarginTop(10);

            // Headers
            table.AddHeaderCell(CreateCell("Kategori", true));
            table.AddHeaderCell(CreateCell("Çalışma Saati", true));
            table.AddHeaderCell(CreateCell("Tamamlanan", true));
            table.AddHeaderCell(CreateCell("Başarı Oranı", true));

            // Data
            foreach (var stat in categoryStats.OrderByDescending(s => s.TotalMinutes))
            {
                table.AddCell(CreateCell(stat.CategoryName));
                table.AddCell(CreateCell($"{stat.Hours:F1}h"));
                table.AddCell(CreateCell(stat.CompletedCount.ToString()));
                table.AddCell(CreateCell($"%{stat.CompletionRate:F1}"));
            }

            document.Add(table);
        }

        private Cell CreateCell(string text, bool isHeader = false)
        {
            var cell = new Cell()
                .Add(new Paragraph(text))
                .SetPadding(8)
                .SetTextAlignment(TextAlignment.CENTER);

            if (isHeader)
            {
                cell.SetBackgroundColor(new DeviceRgb(33, 150, 243))
                    .SetFontColor(ColorConstants.WHITE)
                    .SetBold();
            }
            else
            {
                cell.SetBackgroundColor(new DeviceRgb(250, 250, 250));
            }

            return cell;
        }

        private string GetPriorityText(PriorityLevel priority)
        {
            return priority switch
            {
                PriorityLevel.Low => "Düşük",
                PriorityLevel.Medium => "Orta",
                PriorityLevel.High => "Yüksek",
                _ => "Bilinmiyor"
            };
        }

        #endregion
    }
}

