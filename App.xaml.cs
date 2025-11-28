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
            services.AddSingleton<IAiService>(provider => new GeminiAiService(apiKey));
            services.AddTransient<IPdfService, PdfService>();

            // ViewModels - Transient olarak kaydet (her seferinde yeni instance)
            services.AddTransient<MainViewModel>();
            services.AddTransient<DocumentAnalyzerViewModel>();

            // Views - Transient olarak kaydet
            services.AddTransient<MainWindow>(provider =>
            {
                var viewModel = provider.GetRequiredService<MainViewModel>();
                return new MainWindow(viewModel, provider);
            });
            services.AddTransient<DocumentAnalyzerWindow>();
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


