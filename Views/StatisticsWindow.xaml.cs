using System.Windows;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Views
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(StatisticsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}





