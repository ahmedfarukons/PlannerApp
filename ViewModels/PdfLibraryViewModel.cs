using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.Extensions.DependencyInjection;
using StudyPlanner.Models;
using StudyPlanner.Services;
using StudyPlanner.Interfaces;

namespace StudyPlanner.ViewModels
{
    /// <summary>
    /// PDF Kütüphanesi ViewModel
    /// Kategoriler ve PDF yönetimi
    /// </summary>
    public class PdfLibraryViewModel : ViewModelBase
    {
        private readonly PdfLibraryService _libraryService;
        private readonly IPdfService _pdfService;
        private readonly IAiService _aiService;
        private readonly IDialogService _dialogService;
        private readonly IServiceProvider _serviceProvider;

        private ObservableCollection<Category> _categories;
        private Category? _selectedCategory;
        private PdfDocument? _selectedDocument;
        private bool _isLoading;
        private string _newCategoryName;
        private string _newCategoryIcon;

        public PdfLibraryViewModel(
            PdfLibraryService libraryService,
            IPdfService pdfService,
            IAiService aiService,
            IDialogService dialogService,
            IServiceProvider serviceProvider)
        {
            _libraryService = libraryService ?? throw new ArgumentNullException(nameof(libraryService));
            _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _categories = new ObservableCollection<Category>();
            _newCategoryName = string.Empty;
            _newCategoryIcon = "📁";

            // Commands
            AddCategoryCommand = new RelayCommand(async _ => await AddCategoryAsync(), _ => CanAddCategory());
            DeleteCategoryCommand = new RelayCommand(async _ => await DeleteCategoryAsync(), _ => SelectedCategory != null);
            AddPdfCommand = new RelayCommand(async _ => await AddPdfAsync(), _ => SelectedCategory != null);
            DeletePdfCommand = new RelayCommand(async _ => await DeletePdfAsync(), _ => SelectedDocument != null);
            OpenPdfCommand = new RelayCommand(param => OpenPdf(param as PdfDocument));
            AnalyzePdfCommand = new RelayCommand(param => AnalyzePdf(param as PdfDocument));
            RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());

            // Verileri yükle
            _ = LoadDataAsync();
        }

        #region Properties

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    OnPropertyChanged(nameof(SelectedDocuments));
                }
            }
        }

        public PdfDocument? SelectedDocument
        {
            get => _selectedDocument;
            set => SetProperty(ref _selectedDocument, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string NewCategoryName
        {
            get => _newCategoryName;
            set => SetProperty(ref _newCategoryName, value);
        }

        public string NewCategoryIcon
        {
            get => _newCategoryIcon;
            set => SetProperty(ref _newCategoryIcon, value);
        }

        public ObservableCollection<PdfDocument>? SelectedDocuments => SelectedCategory?.Documents;

        #endregion

        #region Commands

        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand AddPdfCommand { get; }
        public ICommand DeletePdfCommand { get; }
        public ICommand OpenPdfCommand { get; }
        public ICommand AnalyzePdfCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Methods

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                var categories = await _libraryService.LoadCategoriesAsync();
                
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                if (Categories.Count > 0)
                    SelectedCategory = Categories[0];
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Veriler yüklenirken hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddCategoryAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewCategoryName))
                    return;

                var newId = Categories.Any() ? Categories.Max(c => c.Id) + 1 : 1;
                var category = new Category
                {
                    Id = newId,
                    Name = NewCategoryName,
                    Icon = string.IsNullOrWhiteSpace(NewCategoryIcon) ? "📁" : NewCategoryIcon,
                    Color = GetRandomColor()
                };

                Categories.Add(category);
                await SaveDataAsync();

                NewCategoryName = string.Empty;
                NewCategoryIcon = "📁";
                SelectedCategory = category;

                _dialogService.ShowMessage($"'{category.Name}' kategorisi eklendi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Kategori eklenirken hata: {ex.Message}");
            }
        }

        private async Task DeleteCategoryAsync()
        {
            if (SelectedCategory == null)
                return;

            try
            {
                var result = _dialogService.ShowConfirmation(
                    $"'{SelectedCategory.Name}' kategorisini ve içindeki tüm PDF'leri silmek istediğinize emin misiniz?");

                if (!result)
                    return;

                Categories.Remove(SelectedCategory);
                await SaveDataAsync();

                SelectedCategory = Categories.FirstOrDefault();
                _dialogService.ShowMessage("Kategori silindi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Kategori silinirken hata: {ex.Message}");
            }
        }

        private async Task AddPdfAsync()
        {
            if (SelectedCategory == null)
                return;

            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "PDF Dosyaları (*.pdf)|*.pdf",
                    Title = "PDF Seç"
                };

                if (openFileDialog.ShowDialog() != true)
                    return;

                IsLoading = true;

                // Dosya bilgilerini al
                var fileInfo = new FileInfo(openFileDialog.FileName);
                var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

                // PDF'i local kütüphaneye kopyala (opsiyonel)
                var localPath = CopyToLibrary(openFileDialog.FileName);

                var newId = SelectedCategory.Documents.Any() 
                    ? SelectedCategory.Documents.Max(d => d.Id) + 1 
                    : 1;

                var document = new PdfDocument
                {
                    Id = newId,
                    CategoryId = SelectedCategory.Id,
                    Title = fileName,
                    FilePath = localPath, // Local kopyası veya orijinal yol
                    Summary = "Özet henüz oluşturulmadı. Özetlemek için döküman analiz penceresini kullanın.",
                    AddedDate = DateTime.Now,
                    FileSize = fileInfo.Length,
                    PageCount = GetPageCount(openFileDialog.FileName)
                };

                SelectedCategory.Documents.Add(document);
                OnPropertyChanged(nameof(SelectedDocuments));
                await SaveDataAsync();

                SelectedDocument = document;
                _dialogService.ShowMessage($"'{fileName}' eklendi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"PDF eklenirken hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeletePdfAsync()
        {
            if (SelectedDocument == null || SelectedCategory == null)
                return;

            try
            {
                var result = _dialogService.ShowConfirmation(
                    $"'{SelectedDocument.Title}' PDF'ini silmek istediğinize emin misiniz?");

                if (!result)
                    return;

                SelectedCategory.Documents.Remove(SelectedDocument);
                await SaveDataAsync();

                SelectedDocument = null;
                _dialogService.ShowMessage("PDF silindi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"PDF silinirken hata: {ex.Message}");
            }
        }

        private void OpenPdf(PdfDocument? document)
        {
            var doc = document ?? SelectedDocument;
            if (doc == null)
                return;

            try
            {
                if (!File.Exists(doc.FilePath))
                {
                    _dialogService.ShowError("PDF dosyası bulunamadı!");
                    return;
                }

                // Varsayılan PDF okuyucuda aç
                Process.Start(new ProcessStartInfo
                {
                    FileName = doc.FilePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"PDF açılırken hata: {ex.Message}");
            }
        }

        private void AnalyzePdf(PdfDocument? document)
        {
            var doc = document ?? SelectedDocument;
            if (doc == null)
                return;

            try
            {
                if (!File.Exists(doc.FilePath))
                {
                    _dialogService.ShowError("PDF dosyası bulunamadı!");
                    return;
                }

                // DocumentAnalyzer penceresini aç ve PDF'i yükle
                var analyzerWindow = _serviceProvider.GetRequiredService(typeof(Views.DocumentAnalyzerWindow)) as Views.DocumentAnalyzerWindow;
                if (analyzerWindow != null)
                {
                    // ViewModel'e PDF yolunu aktar
                    var viewModel = analyzerWindow.DataContext as DocumentAnalyzerViewModel;
                    if (viewModel != null)
                    {
                        // Pencereyi göster
                        analyzerWindow.Show();
                        // PDF'i otomatik yükle
                        _ = viewModel.LoadPdfFromPathAsync(doc.FilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"PDF analiz edilirken hata: {ex.Message}");
            }
        }

        private async Task SaveDataAsync()
        {
            try
            {
                await _libraryService.SaveCategoriesAsync(Categories.ToList());
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Kaydetme hatası: {ex.Message}");
            }
        }

        private bool CanAddCategory()
        {
            return !string.IsNullOrWhiteSpace(NewCategoryName);
        }

        private int GetPageCount(string pdfPath)
        {
            try
            {
                using (var pdfReader = new iText.Kernel.Pdf.PdfReader(pdfPath))
                using (var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfReader))
                {
                    return pdfDocument.GetNumberOfPages();
                }
            }
            catch
            {
                return 0;
            }
        }

        private string GetRandomColor()
        {
            var colors = new[] { "#2196F3", "#FF5722", "#4CAF50", "#9C27B0", "#FF9800", "#00BCD4", "#E91E63" };
            var random = new Random();
            return colors[random.Next(colors.Length)];
        }

        private string CopyToLibrary(string sourcePath)
        {
            try
            {
                // Local kütüphane klasörü oluştur
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var libraryFolder = Path.Combine(appData, "StudyPlanner", "PdfLibrary");
                
                if (!Directory.Exists(libraryFolder))
                    Directory.CreateDirectory(libraryFolder);

                // Dosyayı kopyala
                var fileName = Path.GetFileName(sourcePath);
                var destPath = Path.Combine(libraryFolder, $"{Guid.NewGuid()}_{fileName}");
                File.Copy(sourcePath, destPath, true);

                return destPath;
            }
            catch
            {
                // Kopyalanamazsa orijinal yolu kullan
                return sourcePath;
            }
        }

        #endregion
    }
}

