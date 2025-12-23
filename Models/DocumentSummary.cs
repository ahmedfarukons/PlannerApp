using System;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Models
{
    /// <summary>
    /// PDF Döküman özeti için model
    /// </summary>
    public class DocumentSummary : ViewModelBase
    {
        private string? _documentId;
        private string _fileName;
        private string _summary;
        private string _modelsUsed;
        private DateTime _uploadDate;
        private long _fileSize;
        private int _pageCount;

        /// <summary>
        /// MongoDB Document Id (PDF kaydı) - opsiyonel
        /// </summary>
        public string? DocumentId
        {
            get => _documentId;
            set => SetProperty(ref _documentId, value);
        }

        /// <summary>
        /// Dosya adı
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        /// <summary>
        /// Döküman özeti
        /// </summary>
        public string Summary
        {
            get => _summary;
            set => SetProperty(ref _summary, value);
        }

        /// <summary>
        /// Kullanılan modeller/algoritmalar
        /// </summary>
        public string ModelsUsed
        {
            get => _modelsUsed;
            set => SetProperty(ref _modelsUsed, value);
        }

        /// <summary>
        /// Yüklenme tarihi
        /// </summary>
        public DateTime UploadDate
        {
            get => _uploadDate;
            set => SetProperty(ref _uploadDate, value);
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
        /// Dosya boyutu formatlanmış (MB/KB)
        /// </summary>
        public string FileSizeDisplay => FormatFileSize(FileSize);

        private string FormatFileSize(long bytes)
        {
            if (bytes >= 1048576) // 1 MB
                return $"{bytes / 1048576.0:F2} MB";
            if (bytes >= 1024) // 1 KB
                return $"{bytes / 1024.0:F2} KB";
            return $"{bytes} bytes";
        }
    }
}

