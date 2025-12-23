using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;
using StudyPlanner.Views;

namespace StudyPlanner.ViewModels
{
    /// <summary>
    /// MongoDB'den kullanıcıya ait PDF + chat geçmişini listeleyen ViewModel.
    /// </summary>
    public class HistoryViewModel : ViewModelBase
    {
        private readonly IUserContext _userContext;
        private readonly IPdfDocumentRepository _pdfRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IDialogService _dialogService;
        private readonly IServiceProvider _serviceProvider;

        private bool _isLoading;
        private MongoPdfDocument? _selectedDocument;
        private string _header = "Geçmiş";

        public ObservableCollection<MongoPdfDocument> Documents { get; } = new ObservableCollection<MongoPdfDocument>();
        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        public MongoPdfDocument? SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                if (SetProperty(ref _selectedDocument, value))
                {
                    _ = LoadChatAsync();
                    OnPropertyChanged(nameof(SelectedSummary));
                    OnPropertyChanged(nameof(SelectedModels));
                    OnPropertyChanged(nameof(SelectedMeta));
                }
            }
        }

        public string SelectedSummary => SelectedDocument?.Summary ?? "";
        public string SelectedModels => SelectedDocument?.ModelsUsed ?? "";
        public string SelectedMeta
        {
            get
            {
                if (SelectedDocument == null) return "";
                var date = SelectedDocument.UploadDateUtc.ToLocalTime().ToString("g");
                return $"{SelectedDocument.FileName} • {SelectedDocument.PageCount} sayfa • {FormatFileSize(SelectedDocument.FileSize)} • {date}";
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand OpenInAnalyzerCommand { get; }

        public HistoryViewModel(
            IUserContext userContext,
            IPdfDocumentRepository pdfRepo,
            IChatRepository chatRepo,
            IDialogService dialogService,
            IServiceProvider serviceProvider)
        {
            _userContext = userContext;
            _pdfRepo = pdfRepo;
            _chatRepo = chatRepo;
            _dialogService = dialogService;
            _serviceProvider = serviceProvider;

            RefreshCommand = new RelayCommand(async _ => await LoadDocumentsAsync(), _ => !IsLoading);
            OpenInAnalyzerCommand = new RelayCommand(_ => OpenInAnalyzer(), _ => SelectedDocument != null && !IsLoading);

            _ = LoadDocumentsAsync();
        }

        private async Task LoadDocumentsAsync()
        {
            if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
            {
                _dialogService.ShowError("Giriş yapılmamış.");
                return;
            }

            try
            {
                IsLoading = true;
                Header = $"Geçmiş ({_userContext.Username})";

                var docs = await _pdfRepo.GetByUserAsync(_userContext.UserId!, limit: 200);
                Documents.Clear();
                foreach (var d in docs)
                    Documents.Add(d);

                SelectedDocument = Documents.Count > 0 ? Documents[0] : null;
                if (SelectedDocument == null)
                    ChatMessages.Clear();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Geçmiş yüklenemedi: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadChatAsync()
        {
            ChatMessages.Clear();
            if (SelectedDocument == null) return;
            if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId)) return;
            if (string.IsNullOrWhiteSpace(SelectedDocument.Id)) return;

            try
            {
                IsLoading = true;
                var msgs = await _chatRepo.GetMessagesAsync(_userContext.UserId!, SelectedDocument.Id!, limit: 500);
                foreach (var m in msgs)
                {
                    ChatMessages.Add(new ChatMessage
                    {
                        IsUser = m.IsUser,
                        Content = m.Content,
                        Timestamp = m.TimestampUtc.ToLocalTime()
                    });
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Chat geçmişi yüklenemedi: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OpenInAnalyzer()
        {
            if (SelectedDocument == null) return;

            try
            {
                var analyzerWindow = _serviceProvider.GetRequiredService<DocumentAnalyzerWindow>();
                analyzerWindow.Show();

                if (analyzerWindow.DataContext is DocumentAnalyzerViewModel vm)
                {
                    // Mongo'dan dokümanı "göster" modunda yükle (chat + summary/models)
                    _ = vm.LoadFromMongoAsync(SelectedDocument.Id!);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Analiz ekranı açılamadı: {ex.Message}");
            }
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes >= 1048576) return $"{bytes / 1048576.0:F2} MB";
            if (bytes >= 1024) return $"{bytes / 1024.0:F2} KB";
            return $"{bytes} bytes";
        }
    }
}


