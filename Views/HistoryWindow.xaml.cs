using System.Windows;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(HistoryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}


