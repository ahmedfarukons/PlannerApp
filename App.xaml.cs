using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;
using StudyPlanner.Repositories;
using StudyPlanner.Services;
using StudyPlanner.ViewModels;
using StudyPlanner.Views;

namespace StudyPlanner
{
    /// <summary>
    /// App.xaml için interaction logic
    /// SOLID: Dependency Inversion - DI Container kullanımı
    /// Clean Architecture - Dependency Injection setup
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        /// <summary>
        /// Application başlangıcında çalışır
        /// Dependency Injection container'ı yapılandırır
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Service Collection oluştur
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Service Provider oluştur
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Temayı yükle
            var themeService = _serviceProvider.GetRequiredService<ThemeService>();
            themeService.LoadSavedTheme();

            // Main window'u göster
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        /// <summary>
        /// Dependency Injection servislerini yapılandırır
        /// SOLID: Dependency Inversion Principle
        /// </summary>
        /// <param name="services">Service collection</param>
        private void ConfigureServices(IServiceCollection services)
        {
            // Repositories - Singleton olarak kaydet
            services.AddSingleton<IRepository<StudyPlanItem>, StudyPlanRepository>();

            // Services - Singleton olarak kaydet
            services.AddSingleton<IDataService<List<StudyPlanItem>>, XmlDataService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Paper Bold AI Services
            // API anahtarı .env veya appsettings.json'dan okunur
            var apiKey = Helpers.ConfigurationHelper.GetGoogleApiKey();
            var apiKeySource = Helpers.ConfigurationHelper.GetGoogleApiKeySource();
            var apiBaseUrl = Helpers.ConfigurationHelper.GetValue("ApiSettings:ApiBaseUrl");
            var temperature = Helpers.ConfigurationHelper.GetDoubleValue("ApiSettings:Temperature", 0.1);
            var maxOutputTokens = Helpers.ConfigurationHelper.GetIntValue("ApiSettings:MaxOutputTokens", 2048);
            var modelCandidates = Helpers.ConfigurationHelper.GetValue("ApiSettings:ModelCandidates");

            services.AddSingleton<IAiService>(_ =>
                new GeminiAiService(
                    apiKey: apiKey,
                    apiBaseUrl: apiBaseUrl,
                    temperature: temperature,
                    maxOutputTokens: maxOutputTokens,
                    apiKeySource: apiKeySource,
                    modelCandidatesCsv: modelCandidates));
            services.AddTransient<IPdfService, PdfService>();

            // PDF Library Service
            services.AddSingleton<PdfLibraryService>();
            
            // Theme Service
            services.AddSingleton<ThemeService>();
            
            // PDF Export Service
            services.AddSingleton<PdfExportService>();

            // ViewModels - Transient olarak kaydet (her seferinde yeni instance)
            services.AddTransient<MainViewModel>();
            services.AddTransient<DocumentAnalyzerViewModel>();
            services.AddTransient<PdfLibraryViewModel>(provider => 
                new PdfLibraryViewModel(
                    provider.GetRequiredService<PdfLibraryService>(),
                    provider.GetRequiredService<IPdfService>(),
                    provider.GetRequiredService<IAiService>(),
                    provider.GetRequiredService<IDialogService>(),
                    provider));
            services.AddTransient<StatisticsViewModel>();

            // Views - Transient olarak kaydet
            services.AddTransient<MainWindow>(provider =>
            {
                var viewModel = provider.GetRequiredService<MainViewModel>();
                var themeService = provider.GetRequiredService<ThemeService>();
                return new MainWindow(viewModel, provider, themeService);
            });
            services.AddTransient<DocumentAnalyzerWindow>();
            services.AddTransient<PdfLibraryWindow>();
            services.AddTransient<StatisticsWindow>();
        }

        /// <summary>
        /// Application kapanırken cleanup yapar
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}


