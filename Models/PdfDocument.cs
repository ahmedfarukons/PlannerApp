using System;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Models
{
    /// <summary>
    /// PDF döküman modeli
    /// </summary>
    public class PdfDocument : ViewModelBase
    {
        private string _title;
        private string _filePath;
        private string _summary;
        private DateTime _addedDate;
        private long _fileSize;
        private int _pageCount;

        public PdfDocument()
        {
            _title = string.Empty;
            _filePath = string.Empty;
            _summary = string.Empty;
            _addedDate = DateTime.Now;
        }

        /// <summary>
        /// Döküman ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Kategori ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Döküman başlığı
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// PDF dosya yolu
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        /// <summary>
        /// 2 cümlelik özet
        /// </summary>
        public string Summary
        {
            get => _summary;
            set => SetProperty(ref _summary, value);
        }

        /// <summary>
        /// Eklenme tarihi
        /// </summary>
        public DateTime AddedDate
        {
            get => _addedDate;
            set => SetProperty(ref _addedDate, value);
        }

        /// <summary>
        /// Dosya boyutu (bytes)
        /// </summary>
        public long FileSize
        {
            get => _fileSize;
            set => SetProperty(ref _fileSize, value);
        }

        /// <summary>
        /// Sayfa sayısı
        /// </summary>
        public int PageCount
        {
            get => _pageCount;
            set => SetProperty(ref _pageCount, value);
        }

        /// <summary>
        /// Tarih gösterimi
        /// </summary>
        public string DateDisplay => AddedDate.ToString("dd.MM.yyyy");

        /// <summary>
        /// Dosya boyutu gösterimi
        /// </summary>
        public string FileSizeDisplay
        {
            get
            {
                if (FileSize >= 1048576) // 1 MB
                    return $"{FileSize / 1048576.0:F1} MB";
                if (FileSize >= 1024) // 1 KB
                    return $"{FileSize / 1024.0:F1} KB";
                return $"{FileSize} bytes";
            }
        }

        /// <summary>
        /// Kısa özet (max 100 karakter)
        /// </summary>
        public string ShortSummary
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Summary))
                    return "Özet yok";
                
                return Summary.Length > 100 
                    ? Summary.Substring(0, 97) + "..." 
                    : Summary;
            }
        }
    }
}



