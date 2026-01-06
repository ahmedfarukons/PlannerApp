using System.Collections.ObjectModel;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Models
{
    /// <summary>
    /// PDF kategorisi (Matematik, Fizik, vb.)
    /// </summary>
    public class Category : ViewModelBase
    {
        private string _name;
        private string _icon;
        private string _color;
        private ObservableCollection<PdfDocument> _documents;

        public Category()
        {
            _name = string.Empty;
            _icon = "📁";
            _color = "#2196F3";
            _documents = new ObservableCollection<PdfDocument>();
            _documents.CollectionChanged += Documents_CollectionChanged;
        }

        /// <summary>
        /// Kategori ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Kategori adı (Matematik, Fizik, vb.)
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Kategori ikonu (emoji)
        /// </summary>
        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        /// <summary>
        /// Kategori rengi (hex)
        /// </summary>
        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        /// <summary>
        /// Bu kategorideki PDF'ler
        /// </summary>
        public ObservableCollection<PdfDocument> Documents
        {
            get => _documents;
            set
            {
                if (_documents != null)
                    _documents.CollectionChanged -= Documents_CollectionChanged;
                
                var newDocuments = value ?? new ObservableCollection<PdfDocument>();
                if (ReferenceEquals(_documents, newDocuments))
                    return;

                _documents = newDocuments;
                OnPropertyChanged();

                _documents.CollectionChanged += Documents_CollectionChanged;
                OnPropertyChanged(nameof(DocumentCount));
            }
        }

        private void Documents_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(DocumentCount));
        }

        /// <summary>
        /// Kategori gösterimi (ikon + ad)
        /// </summary>
        public string DisplayName => $"{Icon} {Name}";

        /// <summary>
        /// Döküman sayısı
        /// </summary>
        public int DocumentCount => Documents?.Count ?? 0;
    }
}


