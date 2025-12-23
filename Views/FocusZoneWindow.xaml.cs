using System.ComponentModel;
using System.Windows;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Views
{
    public partial class FocusZoneWindow : Window
    {
        public FocusZoneWindow(FocusZoneViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            ApplySettingsPanelState(viewModel.IsSettingsOpen);
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            Closed += (_, __) => viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not FocusZoneViewModel vm)
                return;

            if (e.PropertyName == nameof(FocusZoneViewModel.IsSettingsOpen))
                ApplySettingsPanelState(vm.IsSettingsOpen);
        }

        private void ApplySettingsPanelState(bool isOpen)
        {
            if (isOpen)
            {
                SettingsPanel.Visibility = Visibility.Visible;
                SettingsColumn.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                SettingsPanel.Visibility = Visibility.Collapsed;
                SettingsColumn.Width = new GridLength(0);
            }
        }
    }
}


