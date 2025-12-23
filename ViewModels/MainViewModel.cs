using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;

namespace StudyPlanner.ViewModels
{
    /// <summary>
    /// Ana pencere için ViewModel
    /// MVVM Pattern - View ile Model arasındaki köprü
    /// SOLID: Single Responsibility - UI logic sorumluluğu
    /// SOLID: Dependency Inversion - Interface'lere bağımlı
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IRepository<StudyPlanItem> _repository;
        private readonly IDataService<System.Collections.Generic.List<StudyPlanItem>> _dataService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<StudyPlanItem> _studyPlans = new ObservableCollection<StudyPlanItem>();
        private StudyPlanItem? _selectedItem;
        private StudyPlanItem _currentItem = new StudyPlanItem();
        private string _searchText = string.Empty;
        private bool _showCompletedOnly;

        #region Properties

        /// <summary>
        /// Çalışma planları koleksiyonu - UI'a bind edilir
        /// </summary>
        public ObservableCollection<StudyPlanItem> StudyPlans
        {
            get => _studyPlans;
            set => SetProperty(ref _studyPlans, value);
        }

        /// <summary>
        /// Seçili çalışma planı
        /// </summary>
        public StudyPlanItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    // Seçilen item değiştiğinde komutların durumunu güncelle
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Yeni/düzenleme için kullanılan item
        /// </summary>
        public StudyPlanItem CurrentItem
        {
            get => _currentItem;
            set => SetProperty(ref _currentItem, value);
        }

        /// <summary>
        /// Arama metni
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterStudyPlans();
                }
            }
        }

        /// <summary>
        /// Sadece tamamlananları göster
        /// </summary>
        public bool ShowCompletedOnly
        {
            get => _showCompletedOnly;
            set
            {
                if (SetProperty(ref _showCompletedOnly, value))
                {
                    FilterStudyPlans();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Yeni kayıt ekleme komutu
        /// </summary>
        public ICommand AddCommand { get; }

        /// <summary>
        /// Kayıt güncelleme komutu
        /// </summary>
        public ICommand UpdateCommand { get; }

        /// <summary>
        /// Kayıt silme komutu
        /// </summary>
        public ICommand DeleteCommand { get; }

        /// <summary>
        /// Dosyaya kaydetme komutu
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Dosyadan yükleme komutu
        /// </summary>
        public ICommand LoadCommand { get; }

        /// <summary>
        /// Yeni form açma komutu
        /// </summary>
        public ICommand NewCommand { get; }

        /// <summary>
        /// Tamamlama durumunu değiştirme komutu
        /// </summary>
        public ICommand ToggleCompleteCommand { get; }

        #endregion

        /// <summary>
        /// Constructor - Dependency Injection
        /// </summary>
        public MainViewModel(
            IRepository<StudyPlanItem> repository,
            IDataService<System.Collections.Generic.List<StudyPlanItem>> dataService,
            IDialogService dialogService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            StudyPlans = _studyPlans;

            // Commands initialization
            AddCommand = new RelayCommand(async _ => await AddItemAsync(), _ => CanAddItem());
            UpdateCommand = new RelayCommand(async _ => await UpdateItemAsync(), _ => CanUpdateItem());
            DeleteCommand = new RelayCommand(async _ => await DeleteItemAsync(), _ => CanDeleteItem());
            SaveCommand = new RelayCommand(async _ => await SaveToFileAsync());
            LoadCommand = new RelayCommand(async _ => await LoadFromFileAsync());
            NewCommand = new RelayCommand(_ => CreateNewItem());
            ToggleCompleteCommand = new RelayCommand(_ => ToggleComplete(), _ => SelectedItem != null);

            // Yeni item oluştur
            CreateNewItem();

            // Verileri yükle
            _ = LoadDataAsync();
        }

        #region Command Methods

        /// <summary>
        /// Yeni çalışma planı ekler
        /// </summary>
        private async Task AddItemAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentItem.Subject))
                {
                    _dialogService.ShowError("Ders/Konu alanı boş olamaz!");
                    return;
                }

                await _repository.AddAsync(CurrentItem);
                StudyPlans.Add(CurrentItem);
                CreateNewItem();
                _dialogService.ShowMessage("Çalışma planı başarıyla eklendi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ekleme sırasında hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Seçili çalışma planını günceller
        /// </summary>
        private async Task UpdateItemAsync()
        {
            try
            {
                if (SelectedItem == null)
                    return;

                await _repository.UpdateAsync(SelectedItem);
                _dialogService.ShowMessage("Çalışma planı başarıyla güncellendi!");
                
                // UI'ı güncelle
                var index = StudyPlans.IndexOf(SelectedItem);
                if (index >= 0)
                {
                    StudyPlans[index] = SelectedItem;
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Güncelleme sırasında hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Seçili çalışma planını siler
        /// </summary>
        private async Task DeleteItemAsync()
        {
            try
            {
                if (SelectedItem == null)
                    return;

                if (!_dialogService.ShowConfirmation("Seçili çalışma planını silmek istediğinize emin misiniz?"))
                    return;

                await _repository.DeleteAsync(SelectedItem.Id);
                StudyPlans.Remove(SelectedItem);
                SelectedItem = null;
                _dialogService.ShowMessage("Çalışma planı başarıyla silindi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Silme sırasında hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Dosyaya kaydeder
        /// </summary>
        private async Task SaveToFileAsync()
        {
            try
            {
                var filePath = _dialogService.ShowSaveFileDialog();
                if (string.IsNullOrEmpty(filePath))
                    return;

                var items = await _repository.GetAllAsync();
                await _dataService.SaveToFileAsync(items.ToList(), filePath);
                _dialogService.ShowMessage("Veriler başarıyla kaydedildi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Kaydetme sırasında hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Dosyadan yükler
        /// </summary>
        private async Task LoadFromFileAsync()
        {
            try
            {
                var filePath = _dialogService.ShowOpenFileDialog();
                if (string.IsNullOrEmpty(filePath))
                    return;

                var items = await _dataService.LoadFromFileAsync(filePath);
                await LoadItemsToRepository(items);
                _dialogService.ShowMessage($"{items.Count} kayıt başarıyla yüklendi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Yükleme sırasında hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Yeni item oluşturur
        /// </summary>
        private void CreateNewItem()
        {
            CurrentItem = new StudyPlanItem
            {
                Date = DateTime.Now,
                DurationMinutes = 30,
                Priority = PriorityLevel.Medium
            };
        }

        /// <summary>
        /// Tamamlanma durumunu değiştirir
        /// </summary>
        private void ToggleComplete()
        {
            if (SelectedItem != null)
            {
                SelectedItem.IsCompleted = !SelectedItem.IsCompleted;
            }
        }

        #endregion

        #region Can Execute Methods

        private bool CanAddItem()
        {
            return CurrentItem != null && !string.IsNullOrWhiteSpace(CurrentItem.Subject);
        }

        private bool CanUpdateItem()
        {
            return SelectedItem != null;
        }

        private bool CanDeleteItem()
        {
            return SelectedItem != null;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// İlk yüklemede verileri getirir
        /// </summary>
        private async Task LoadDataAsync()
        {
            try
            {
                var items = await _dataService.LoadAsync();
                await LoadItemsToRepository(items);
            }
            catch
            {
                // İlk çalıştırmada dosya olmayabilir, sessizce devam et
            }
        }

        /// <summary>
        /// Item'ları repository'e yükler
        /// </summary>
        private async Task LoadItemsToRepository(System.Collections.Generic.List<StudyPlanItem> items)
        {
            if (items == null || items.Count == 0)
                return;

            StudyPlans.Clear();
            
            // Repository'yi temizle
            var existingItems = await _repository.GetAllAsync();
            foreach (var item in existingItems.ToList())
            {
                await _repository.DeleteAsync(item.Id);
            }

            // Yeni item'ları ekle
            foreach (var item in items)
            {
                await _repository.AddAsync(item);
                StudyPlans.Add(item);
            }
        }

        /// <summary>
        /// Çalışma planlarını filtreler
        /// </summary>
        private async void FilterStudyPlans()
        {
            var items = await _repository.GetAllAsync();

            // Tamamlanma filtreleme
            if (ShowCompletedOnly)
            {
                items = items.Where(x => x.IsCompleted);
            }

            // Arama filtreleme
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                items = items.Where(x =>
                    x.Subject.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    (x.Notes != null && x.Notes.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Category != null && x.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
            }

            StudyPlans.Clear();
            foreach (var item in items.OrderByDescending(x => x.Date))
            {
                StudyPlans.Add(item);
            }
        }

        #endregion
    }
}



