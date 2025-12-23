using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;
using StudyPlanner.Services;

namespace StudyPlanner.ViewModels
{
    /// <summary>
    /// İstatistikler ViewModel
    /// Çalışma planı analiz ve raporlama
    /// </summary>
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly IRepository<StudyPlanItem> _repository;
        private readonly IDialogService _dialogService;
        private readonly SPdfExportService _exportService;

        private ObservableCollection<StudyStatistic> _weeklyStats;
        private ObservableCollection<CategoryStatistic> _categoryStats;
        private ObservableCollection<PriorityStatistic> _priorityStats;
        private int _totalStudyMinutes;
        private int _completedPlans;
        private int _totalPlans;
        private double _completionRate;
        private string _mostStudiedCategory;
        private bool _isLoading;

        public StatisticsViewModel(
            IRepository<StudyPlanItem> repository,
            IDialogService dialogService,
            SPdfExportService exportService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));

            _weeklyStats = new ObservableCollection<StudyStatistic>();
            _categoryStats = new ObservableCollection<CategoryStatistic>();
            _priorityStats = new ObservableCollection<PriorityStatistic>();
            _mostStudiedCategory = "Yok";

            // Commands
            RefreshCommand = new RelayCommand(async _ => await LoadStatisticsAsync());
            ExportCommand = new RelayCommand(async _ => await ExportStatisticsAsync());

            // Load data
            _ = LoadStatisticsAsync();
        }

        #region Properties

        public ObservableCollection<StudyStatistic> WeeklyStats
        {
            get => _weeklyStats;
            set => SetProperty(ref _weeklyStats, value);
        }

        public ObservableCollection<CategoryStatistic> CategoryStats
        {
            get => _categoryStats;
            set => SetProperty(ref _categoryStats, value);
        }

        public ObservableCollection<PriorityStatistic> PriorityStats
        {
            get => _priorityStats;
            set => SetProperty(ref _priorityStats, value);
        }

        public int TotalStudyMinutes
        {
            get => _totalStudyMinutes;
            set
            {
                if (SetProperty(ref _totalStudyMinutes, value))
                {
                    OnPropertyChanged(nameof(TotalStudyHours));
                }
            }
        }

        public int CompletedPlans
        {
            get => _completedPlans;
            set => SetProperty(ref _completedPlans, value);
        }

        public int TotalPlans
        {
            get => _totalPlans;
            set => SetProperty(ref _totalPlans, value);
        }

        public double CompletionRate
        {
            get => _completionRate;
            set => SetProperty(ref _completionRate, value);
        }

        public string MostStudiedCategory
        {
            get => _mostStudiedCategory;
            set => SetProperty(ref _mostStudiedCategory, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public double TotalStudyHours => TotalStudyMinutes / 60.0;

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand ExportCommand { get; }

        #endregion

        #region Methods

        public async Task LoadStatisticsAsync()
        {
            try
            {
                IsLoading = true;

                var allPlans = (await _repository.GetAllAsync()).ToList();

                // Genel istatistikler
                TotalPlans = allPlans.Count;
                CompletedPlans = allPlans.Count(p => p.IsCompleted);
                CompletionRate = TotalPlans > 0 ? (double)CompletedPlans / TotalPlans * 100 : 0;
                TotalStudyMinutes = allPlans.Where(p => p.IsCompleted).Sum(p => p.DurationMinutes);

                // Haftalık istatistikler (son 7 gün)
                CalculateWeeklyStats(allPlans);

                // Kategori istatistikleri
                CalculateCategoryStats(allPlans);

                // Öncelik istatistikleri
                CalculatePriorityStats(allPlans);

                // En çok çalışılan kategori
                MostStudiedCategory = CategoryStats.OrderByDescending(c => c.TotalMinutes)
                    .FirstOrDefault()?.CategoryName ?? "Yok";
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"İstatistikler yüklenirken hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CalculateWeeklyStats(List<StudyPlanItem> allPlans)
        {
            WeeklyStats.Clear();

            var today = DateTime.Today;
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var dayPlans = allPlans.Where(p => p.Date.Date == date).ToList();
                
                WeeklyStats.Add(new StudyStatistic
                {
                    DayName = date.ToString("ddd"),
                    Date = date,
                    TotalMinutes = dayPlans.Where(p => p.IsCompleted).Sum(p => p.DurationMinutes),
                    PlanCount = dayPlans.Count,
                    CompletedCount = dayPlans.Count(p => p.IsCompleted)
                });
            }
        }

        private void CalculateCategoryStats(List<StudyPlanItem> allPlans)
        {
            CategoryStats.Clear();

            var categoryGroups = allPlans
                .Where(p => !string.IsNullOrWhiteSpace(p.Category))
                .GroupBy(p => p.Category)
                .OrderByDescending(g => g.Sum(p => p.IsCompleted ? p.DurationMinutes : 0));

            foreach (var group in categoryGroups)
            {
                var completedPlans = group.Where(p => p.IsCompleted).ToList();
                CategoryStats.Add(new CategoryStatistic
                {
                    CategoryName = group.Key,
                    TotalMinutes = completedPlans.Sum(p => p.DurationMinutes),
                    PlanCount = group.Count(),
                    CompletedCount = completedPlans.Count,
                    Color = GetCategoryColor(group.Key)
                });
            }
        }

        private void CalculatePriorityStats(List<StudyPlanItem> allPlans)
        {
            PriorityStats.Clear();

            var priorityGroups = allPlans.GroupBy(p => p.Priority);

            foreach (var group in priorityGroups)
            {
                var completedPlans = group.Where(p => p.IsCompleted).ToList();
                PriorityStats.Add(new PriorityStatistic
                {
                    Priority = group.Key,
                    PriorityName = GetPriorityName(group.Key),
                    TotalMinutes = completedPlans.Sum(p => p.DurationMinutes),
                    PlanCount = group.Count(),
                    CompletedCount = completedPlans.Count,
                    Color = GetPriorityColor(group.Key)
                });
            }
        }

        private string GetPriorityName(PriorityLevel priority)
        {
            return priority switch
            {
                PriorityLevel.Low => "Düşük",
                PriorityLevel.Medium => "Orta",
                PriorityLevel.High => "Yüksek",
                _ => "Bilinmiyor"
            };
        }

        private string GetPriorityColor(PriorityLevel priority)
        {
            return priority switch
            {
                PriorityLevel.Low => "#4CAF50",
                PriorityLevel.Medium => "#FF9800",
                PriorityLevel.High => "#F44336",
                _ => "#9E9E9E"
            };
        }

        private string GetCategoryColor(string category)
        {
            var hash = category.GetHashCode();
            var colors = new[] { "#2196F3", "#FF5722", "#4CAF50", "#9C27B0", "#FF9800", "#00BCD4", "#E91E63" };
            return colors[Math.Abs(hash) % colors.Length];
        }

        private async Task ExportStatisticsAsync()
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF Dosyası (*.pdf)|*.pdf",
                    FileName = $"İstatistikler_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "İstatistikleri Kaydet"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                IsLoading = true;

                await _exportService.ExportStatisticsAsync(
                    TotalPlans,
                    CompletedPlans,
                    CompletionRate,
                    TotalStudyHours,
                    CategoryStats.ToList(),
                    WeeklyStats.ToList(),
                    saveFileDialog.FileName
                );

                _dialogService.ShowMessage("İstatistikler başarıyla PDF'e aktarıldı!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Export hatası: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }

    #region Statistics Models

    /// <summary>
    /// Günlük çalışma istatistiği
    /// </summary>
    public class StudyStatistic
    {
        public string DayName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int TotalMinutes { get; set; }
        public int PlanCount { get; set; }
        public int CompletedCount { get; set; }
        public double Hours => TotalMinutes / 60.0;
    }

    /// <summary>
    /// Kategori istatistiği
    /// </summary>
    public class CategoryStatistic
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TotalMinutes { get; set; }
        public int PlanCount { get; set; }
        public int CompletedCount { get; set; }
        public string Color { get; set; } = "#2196F3";
        public double Hours => TotalMinutes / 60.0;
        public double CompletionRate => PlanCount > 0 ? (double)CompletedCount / PlanCount * 100 : 0;
    }

    /// <summary>
    /// Öncelik istatistiği
    /// </summary>
    public class PriorityStatistic
    {
        public PriorityLevel Priority { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public int TotalMinutes { get; set; }
        public int PlanCount { get; set; }
        public int CompletedCount { get; set; }
        public string Color { get; set; } = "#2196F3";
        public double Hours => TotalMinutes / 60.0;
    }

    #endregion
}

