using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StudyPlanner.ViewModels;
using StudyPlanner.Services;

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
        private readonly ThemeService _themeService;
        private bool _isHeaderCollapsed;

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
        /// <param name="themeService">Theme service for dark mode</param>
        public MainWindow(MainViewModel viewModel, IServiceProvider serviceProvider, ThemeService themeService) : this()
        {
            DataContext = viewModel;
            _serviceProvider = serviceProvider;
            _themeService = themeService;
        }

        private void HeaderNotch_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isHeaderCollapsed = !_isHeaderCollapsed;

            if (_isHeaderCollapsed)
            {
                HeaderBar.Visibility = Visibility.Collapsed;
                HeaderRow.Height = new GridLength(0);
                HeaderNotchText.Text = "⌄";
            }
            else
            {
                HeaderBar.Visibility = Visibility.Visible;
                HeaderRow.Height = GridLength.Auto;
                HeaderNotchText.Text = "⌃";
            }
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

        /// <summary>
        /// PDF Kütüphanesi penceresini açar
        /// </summary>
        private void PdfLibrary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var libraryWindow = _serviceProvider.GetRequiredService<PdfLibraryWindow>();
                libraryWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PDF Kütüphanesi penceresi açılırken hata: {ex.Message}", 
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// MongoDB Geçmiş penceresini açar (PDF + chat kayıtları)
        /// </summary>
        private void History_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var historyWindow = _serviceProvider.GetRequiredService<HistoryWindow>();
                historyWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Geçmiş penceresi açılırken hata: {ex.Message}",
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Focus Zone (Pomodoro) penceresini açar
        /// </summary>
        private void FocusZone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var focusWindow = _serviceProvider.GetRequiredService<FocusZoneWindow>();
                focusWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Focus Zone açılırken hata: {ex.Message}",
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// İstatistikler penceresini açar
        /// </summary>
        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var statisticsWindow = _serviceProvider.GetRequiredService<StatisticsWindow>();
                statisticsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İstatistikler penceresi açılırken hata: {ex.Message}", 
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tema değiştirme (Light/Dark Mode)
        /// </summary>
        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _themeService.ToggleTheme();
                
                var currentTheme = _themeService.CurrentTheme;
                MessageBox.Show($"{currentTheme} tema aktif edildi!", 
                    "Tema Değişti", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tema değiştirme hatası: {ex.Message}", 
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}



