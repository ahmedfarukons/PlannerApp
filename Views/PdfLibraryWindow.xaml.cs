using System.Windows;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Views
{
    /// <summary>
    /// PdfLibraryWindow.xaml için interaction logic
    /// </summary>
    public partial class PdfLibraryWindow : Window
    {
        public PdfLibraryWindow(PdfLibraryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}





