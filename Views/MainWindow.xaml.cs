using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Views
{
    /// <summary>
    /// MainWindow code-behind
    /// MVVM Pattern - View katmanı
    /// Minimal code-behind, logic ViewModel'de
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor
        /// DataContext Dependency Injection ile set edilecek
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with ViewModel injection
        /// </summary>
        /// <param name="viewModel">MainViewModel instance</param>
        /// <param name="serviceProvider">Service provider for creating new windows</param>
        public MainWindow(MainViewModel viewModel, IServiceProvider serviceProvider) : this()
        {
            DataContext = viewModel;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Döküman Analiz penceresini açar
        /// </summary>
        private void DocumentAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var documentWindow = _serviceProvider.GetRequiredService<DocumentAnalyzerWindow>();
                documentWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Döküman Analiz penceresi açılırken hata: {ex.Message}", 
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}



