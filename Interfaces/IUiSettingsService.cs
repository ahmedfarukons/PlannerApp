using System.ComponentModel;

namespace StudyPlanner.Interfaces
{
    public interface IUiSettingsService : INotifyPropertyChanged
    {
        double UiScale { get; set; }
    }
}
