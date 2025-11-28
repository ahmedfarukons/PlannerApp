using System.Windows;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Views
{
    /// <summary>
    /// DocumentAnalyzerWindow.xaml için interaction logic
    /// </summary>
    public partial class DocumentAnalyzerWindow : Window
    {
        public DocumentAnalyzerWindow(DocumentAnalyzerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

