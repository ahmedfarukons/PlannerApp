using System.ComponentModel;
using System.Runtime.CompilerServices;
using StudyPlanner.Interfaces;

namespace StudyPlanner.Services
{
    public class SUiSettingsService : IUiSettingsService
    {
        private double _uiScale = 1.0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public double UiScale
        {
            get => _uiScale;
            set
            {
                if (_uiScale != value)
                {
                    _uiScale = value;
                    OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
