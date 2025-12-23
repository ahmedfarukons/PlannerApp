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
        private ServiceProvider? _serviceProvider;

        /// <summary>
        /// Application başlangıcında çalışır
        /// Dependency Injection container'ı yapılandırır
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // Login penceresi kapanınca (MainWindow açılmadan önce) uygulama otomatik kapanmasın.
                // Varsayılan ShutdownMode: OnLastWindowClose -> Login kapanınca app kapanabiliyor.
                ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // Service Collection oluştur
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                // Service Provider oluştur
                _serviceProvider = serviceCollection.BuildServiceProvider();

                // Temayı yükle
                var themeService = _serviceProvider.GetRequiredService<SThemeService>();
                themeService.LoadSavedTheme();

                // "Beni hatırla" -> auto login
                var credentialStore = _serviceProvider.GetRequiredService<SAuthCredentialStore>();
                var userService = _serviceProvider.GetRequiredService<IUserService>();
                if (credentialStore.TryLoad(out var savedIdentifier, out var savedPassword))
                {
                    try
                    {
                        var auto = await userService.LoginAsync(savedIdentifier, savedPassword);
                        if (!auto.Success)
                            credentialStore.Clear();
                    }
                    catch
                    {
                        credentialStore.Clear();
                    }
                }

                var userContext = _serviceProvider.GetRequiredService<IUserContext>();

                // Login -> Main akışı (gerekirse)
                if (!userContext.IsAuthenticated)
                {
                    var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
                    if (loginWindow.DataContext is LoginViewModel lvm)
                        lvm.LoginIdentifier = savedIdentifier ?? string.Empty;

                    var loginOk = loginWindow.ShowDialog() == true;
                    if (!loginOk)
                    {
                        Shutdown();
                        return;
                    }
                }

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                MainWindow = mainWindow;
                mainWindow.Show();

                // Artık ana pencere var; app, MainWindow kapanınca kapansın.
                ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Uygulama başlatılamadı: {ex.Message}\n\n" +
                    "MongoDB için MONGO_CONNECTION_STRING ve MONGO_DATABASE ayarlarını kontrol edin.",
                    "Başlatma Hatası",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
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
            services.AddSingleton<IDataService<List<StudyPlanItem>>, SXmlDataService>();
            services.AddSingleton<IDialogService, SDialogService>();

            // MongoDB
            var mongoConn = Helpers.ConfigurationHelper.GetMongoConnectionString();
            var mongoDbName = Helpers.ConfigurationHelper.GetMongoDatabaseName();
            services.AddSingleton(_ => new MongoDbContext(mongoConn, mongoDbName));
            services.AddSingleton<MongoUserRepository>();
            services.AddSingleton<IPdfDocumentRepository, MongoPdfDocumentRepository>();
            services.AddSingleton<IChatRepository, MongoChatRepository>();

            // Auth/session
            services.AddSingleton<IUserContext, SUserContext>();
            services.AddSingleton<IUserService, SUserService>();
            services.AddSingleton<SAuthCredentialStore>();

            // Paper Bold AI Services
            // API anahtarı .env veya appsettings.json'dan okunur
            var apiKey = Helpers.ConfigurationHelper.GetGoogleApiKey();
            var apiKeySource = Helpers.ConfigurationHelper.GetGoogleApiKeySource();
            var apiBaseUrl = Helpers.ConfigurationHelper.GetValue("ApiSettings:ApiBaseUrl");
            var temperature = Helpers.ConfigurationHelper.GetDoubleValue("ApiSettings:Temperature", 0.1);
            var maxOutputTokens = Helpers.ConfigurationHelper.GetIntValue("ApiSettings:MaxOutputTokens", 2048);
            var modelCandidates = Helpers.ConfigurationHelper.GetValue("ApiSettings:ModelCandidates");

            services.AddSingleton<IAiService>(_ =>
                new SGeminiAiService(
                    apiKey: apiKey,
                    apiBaseUrl: apiBaseUrl,
                    temperature: temperature,
                    maxOutputTokens: maxOutputTokens,
                    apiKeySource: apiKeySource,
                    modelCandidatesCsv: modelCandidates));
            services.AddTransient<IPdfService, SPdfService>();

            // PDF Library Service
            services.AddSingleton<SPdfLibraryService>();
            
            // Theme Service
            services.AddSingleton<SThemeService>();
            
            // PDF Export Service
            services.AddSingleton<SPdfExportService>();

            // ViewModels - Transient olarak kaydet (her seferinde yeni instance)
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<DocumentAnalyzerViewModel>();
            services.AddTransient<HistoryViewModel>();
            services.AddTransient<FocusZoneViewModel>();
            services.AddTransient<PdfLibraryViewModel>(provider => 
                new PdfLibraryViewModel(
                    provider.GetRequiredService<SPdfLibraryService>(),
                    provider.GetRequiredService<IPdfService>(),
                    provider.GetRequiredService<IAiService>(),
                    provider.GetRequiredService<IDialogService>(),
                    provider));
            services.AddTransient<StatisticsViewModel>();

            // Views - Transient olarak kaydet
            services.AddTransient<MainWindow>(provider =>
            {
                var viewModel = provider.GetRequiredService<MainViewModel>();
                var themeService = provider.GetRequiredService<SThemeService>();
                return new MainWindow(viewModel, provider, themeService);
            });

            services.AddTransient<LoginWindow>();
            services.AddTransient<HistoryWindow>();
            services.AddTransient<FocusZoneWindow>();
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


