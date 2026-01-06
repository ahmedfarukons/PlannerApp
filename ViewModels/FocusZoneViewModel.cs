using System;
using System.Windows.Input;
using System.Windows.Threading;
using System.Linq;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;

namespace StudyPlanner.ViewModels
{
    /// <summary>
    /// Pomodoro/odak modu için ViewModel (geri sayım + süre ayarı).
    /// </summary>
    public class FocusZoneViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IRepository<StudyPlanItem> _repository;
        private readonly DispatcherTimer _timer;

        private int _durationMinutes = 25;
        private TimeSpan _remaining;
        private bool _isRunning;
        private bool _isSettingsOpen = true;
        private StudyPlanItem? _selectedTask;
        private System.Collections.ObjectModel.ObservableCollection<StudyPlanItem> _studyPlans;

        public FocusZoneViewModel(IDialogService dialogService, IRepository<StudyPlanItem> repository)
        {
            _dialogService = dialogService;
            _repository = repository;

            _studyPlans = new System.Collections.ObjectModel.ObservableCollection<StudyPlanItem>();
            _remaining = TimeSpan.FromMinutes(_durationMinutes);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += async (_, __) => await TickAsync();

            StartPauseCommand = new RelayCommand(_ => ToggleStartPause());
            ResetCommand = new RelayCommand(_ => Reset(), _ => !IsRunning && Remaining != TimeSpan.FromMinutes(DurationMinutes));
            SetPreset25Command = new RelayCommand(_ => SetDuration(25), _ => !IsRunning);
            SetPreset50Command = new RelayCommand(_ => SetDuration(50), _ => !IsRunning);
            SetPreset90Command = new RelayCommand(_ => SetDuration(90), _ => !IsRunning);
            ToggleSettingsCommand = new RelayCommand(_ => IsSettingsOpen = !IsSettingsOpen);

            // Load incomplete tasks
            _ = LoadTasksAsync();
        }

        public System.Collections.ObjectModel.ObservableCollection<StudyPlanItem> StudyPlans
        {
            get => _studyPlans;
            set => SetProperty(ref _studyPlans, value);
        }

        public StudyPlanItem? SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        public int DurationMinutes
        {
            get => _durationMinutes;
            set
            {
                var v = Math.Max(1, Math.Min(180, value));
                if (SetProperty(ref _durationMinutes, v))
                {
                    if (!IsRunning)
                    {
                        Remaining = TimeSpan.FromMinutes(_durationMinutes);
                        OnPropertyChanged(nameof(ProgressPercent));
                    }
                }
            }
        }

        public TimeSpan Remaining
        {
            get => _remaining;
            private set
            {
                if (SetProperty(ref _remaining, value))
                {
                    OnPropertyChanged(nameof(RemainingDisplay));
                    OnPropertyChanged(nameof(ProgressPercent));
                }
            }
        }

        public string RemainingDisplay => $"{(int)Remaining.TotalMinutes:00}:{Remaining.Seconds:00}";

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (SetProperty(ref _isRunning, value))
                {
                    OnPropertyChanged(nameof(StartPauseLabel));
                }
            }
        }

        public bool IsSettingsOpen
        {
            get => _isSettingsOpen;
            set => SetProperty(ref _isSettingsOpen, value);
        }

        public string StartPauseLabel => IsRunning ? "⏸ Duraklat" : "▶ Başlat / Devam";

        public double ProgressPercent
        {
            get
            {
                var total = TimeSpan.FromMinutes(DurationMinutes).TotalSeconds;
                if (total <= 0) return 0;
                var elapsed = total - Remaining.TotalSeconds;
                return Math.Max(0, Math.Min(100, (elapsed / total) * 100));
            }
        }

        public ICommand StartPauseCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand SetPreset25Command { get; }
        public ICommand SetPreset50Command { get; }
        public ICommand SetPreset90Command { get; }
        public ICommand ToggleSettingsCommand { get; }

        private async System.Threading.Tasks.Task LoadTasksAsync()
        {
            try
            {
                var items = await _repository.GetAllAsync();
                var incomplete = items.Where(x => !x.IsCompleted).OrderBy(x => x.Date).ToList();

                StudyPlans.Clear();
                foreach (var item in incomplete)
                {
                    StudyPlans.Add(item);
                }
            }
            catch { /* Ignore errors during initial load */ }
        }

        private void SetDuration(int minutes)
        {
            DurationMinutes = minutes;
            Remaining = TimeSpan.FromMinutes(DurationMinutes);
        }

        private void ToggleStartPause()
        {
            if (IsRunning)
            {
                _timer.Stop();
                IsRunning = false;
                return;
            }

            if (Remaining.TotalSeconds <= 0)
                Remaining = TimeSpan.FromMinutes(DurationMinutes);

            IsRunning = true;
            IsSettingsOpen = false;
            _timer.Start();
        }

        private void Reset()
        {
            _timer.Stop();
            IsRunning = false;
            Remaining = TimeSpan.FromMinutes(DurationMinutes);
        }

        private async System.Threading.Tasks.Task TickAsync()
        {
            if (!IsRunning) return;

            var next = Remaining - TimeSpan.FromSeconds(1);
            if (next.TotalSeconds <= 0)
            {
                Remaining = TimeSpan.Zero;
                _timer.Stop();
                IsRunning = false;

                // Update task duration
                if (SelectedTask != null)
                {
                    try
                    {
                        SelectedTask.ActualDurationMinutes += DurationMinutes;
                        await _repository.UpdateAsync(SelectedTask);
                        _dialogService.ShowMessage($"Süre doldu! '{SelectedTask.Subject}' için {DurationMinutes} dk eklendi. 🎯", "Focus Zone");
                    }
                    catch
                    {
                        _dialogService.ShowMessage("Süre doldu! (Görev süresi güncellenirken hata oluştu)", "Focus Zone");
                    }
                }
                else
                {
                    _dialogService.ShowMessage("Süre doldu! 🎯", "Focus Zone");
                }
                
                return;
            }

            Remaining = next;
        }
    }
}


