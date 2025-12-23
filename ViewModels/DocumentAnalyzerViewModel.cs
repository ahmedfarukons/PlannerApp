using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;

namespace StudyPlanner.ViewModels
{
    /// <summary>
    /// Döküman analiz ekranı için ViewModel
    /// Paper Bold entegrasyonu
    /// </summary>
    public class DocumentAnalyzerViewModel : ViewModelBase
    {
        private readonly IPdfService _pdfService;
        private readonly IAiService _aiService;
        private readonly IDialogService _dialogService;
        private readonly IUserContext _userContext;
        private readonly IChatRepository _chatRepository;
        private readonly IPdfDocumentRepository _pdfRepository;

        private string? _currentDocumentId;

        private DocumentSummary _currentDocument = new DocumentSummary();
        private string _pdfText = string.Empty;
        private string _questionText = string.Empty;
        private bool _isProcessing;
        private bool _hasDocument;
        private ObservableCollection<ChatMessage> _chatMessages = new ObservableCollection<ChatMessage>();

        #region Properties

        /// <summary>
        /// Mevcut döküman özeti
        /// </summary>
        public DocumentSummary CurrentDocument
        {
            get => _currentDocument;
            set => SetProperty(ref _currentDocument, value);
        }

        /// <summary>
        /// PDF tam metin (soru-cevap için kullanılır)
        /// </summary>
        public string PdfText
        {
            get => _pdfText;
            set => SetProperty(ref _pdfText, value);
        }

        /// <summary>
        /// Kullanıcı sorusu
        /// </summary>
        public string QuestionText
        {
            get => _questionText;
            set => SetProperty(ref _questionText, value);
        }

        /// <summary>
        /// İşlem yapılıyor mu
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        /// <summary>
        /// Döküman yüklenmiş mi
        /// </summary>
        public bool HasDocument
        {
            get => _hasDocument;
            set => SetProperty(ref _hasDocument, value);
        }

        /// <summary>
        /// Sohbet mesajları
        /// </summary>
        public ObservableCollection<ChatMessage> ChatMessages
        {
            get => _chatMessages;
            set => SetProperty(ref _chatMessages, value);
        }

        #endregion

        #region Commands

        /// <summary>
        /// PDF yükleme komutu
        /// </summary>
        public ICommand UploadPdfCommand { get; }

        /// <summary>
        /// Soru sorma komutu
        /// </summary>
        public ICommand AskQuestionCommand { get; }

        /// <summary>
        /// Yeni döküman yükleme komutu
        /// </summary>
        public ICommand NewDocumentCommand { get; }

        #endregion

        public DocumentAnalyzerViewModel(
            IPdfService pdfService,
            IAiService aiService,
            IDialogService dialogService,
            IUserContext userContext,
            IChatRepository chatRepository,
            IPdfDocumentRepository pdfRepository)
        {
            _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            _pdfRepository = pdfRepository ?? throw new ArgumentNullException(nameof(pdfRepository));

            // Defaults already set on fields; keep property assignment for binding update
            ChatMessages = _chatMessages;

            // Commands
            UploadPdfCommand = new RelayCommand(async _ => await UploadPdfAsync(), _ => !IsProcessing);
            AskQuestionCommand = new RelayCommand(async _ => await AskQuestionAsync(), _ => CanAskQuestion());
            NewDocumentCommand = new RelayCommand(_ => ResetDocument(), _ => HasDocument);
        }

        #region Command Methods

        /// <summary>
        /// Dışarıdan verilen path ile PDF yükle
        /// </summary>
        public async Task LoadPdfFromPathAsync(string pdfPath)
        {
            try
            {
                if (!System.IO.File.Exists(pdfPath))
                {
                    _dialogService.ShowError("PDF dosyası bulunamadı!");
                    return;
                }

                IsProcessing = true;

                // PDF'i işle
                CurrentDocument = await _pdfService.ProcessPdfAsync(pdfPath);
                PdfText = await _pdfService.ExtractTextAsync(pdfPath);
                _currentDocumentId = CurrentDocument.DocumentId;

                HasDocument = true;

                _dialogService.ShowMessage("PDF başarıyla analiz edildi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"PDF yükleme hatası: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// PDF dosyası yükler ve analiz eder
        /// </summary>
        private async Task UploadPdfAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "PDF Dosyaları (*.pdf)|*.pdf",
                    Title = "PDF Dosyası Seçin"
                };

                if (openFileDialog.ShowDialog() != true)
                    return;

                IsProcessing = true;

                // PDF'i işle
                CurrentDocument = await _pdfService.ProcessPdfAsync(openFileDialog.FileName);
                PdfText = await _pdfService.ExtractTextAsync(openFileDialog.FileName);
                _currentDocumentId = CurrentDocument.DocumentId;

                HasDocument = true;

                _dialogService.ShowMessage("PDF başarıyla analiz edildi!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"PDF yükleme hatası: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Döküman hakkında soru sorar
        /// </summary>
        private async Task AskQuestionAsync()
        {
            if (string.IsNullOrWhiteSpace(QuestionText))
                return;

            try
            {
                IsProcessing = true;

                // Kullanıcı mesajını ekle
                var userMsg = new ChatMessage
                {
                    IsUser = true,
                    Content = QuestionText,
                    Timestamp = DateTime.Now
                };
                ChatMessages.Add(userMsg);

                // Mongo'ya kaydet (kullanıcı + doküman varsa)
                await TryPersistChatAsync(isUser: true, content: userMsg.Content, timestampLocal: userMsg.Timestamp);

                var question = QuestionText;
                QuestionText = string.Empty;

                // AI'dan cevap al
                var answer = await _aiService.AskQuestionAsync(question, PdfText);

                // AI cevabını ekle
                var aiMsg = new ChatMessage
                {
                    IsUser = false,
                    Content = answer,
                    Timestamp = DateTime.Now
                };
                ChatMessages.Add(aiMsg);
                await TryPersistChatAsync(isUser: false, content: aiMsg.Content, timestampLocal: aiMsg.Timestamp);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Soru cevaplama hatası: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Dökümanı sıfırlar
        /// </summary>
        private void ResetDocument()
        {
            CurrentDocument = new DocumentSummary();
            PdfText = string.Empty;
            QuestionText = string.Empty;
            HasDocument = false;
            _currentDocumentId = null;
            ChatMessages.Clear();
        }

        private async Task TryPersistChatAsync(bool isUser, string content, DateTime timestampLocal)
        {
            try
            {
                if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
                    return;
                if (string.IsNullOrWhiteSpace(_currentDocumentId))
                    return;

                await _chatRepository.AddMessageAsync(
                    userId: _userContext.UserId!,
                    documentId: _currentDocumentId!,
                    isUser: isUser,
                    content: content,
                    timestampUtc: timestampLocal.ToUniversalTime());
            }
            catch
            {
                // Persist hatası UI'yı bozmasın
            }
        }

        #endregion

        #region Can Execute Methods

        private bool CanAskQuestion()
        {
            // PdfText yoksa (örn: Mongo'dan sadece geçmiş gösteriminde) soru sormayı kapat.
            return HasDocument && !IsProcessing && !string.IsNullOrWhiteSpace(QuestionText) && !string.IsNullOrWhiteSpace(PdfText);
        }

        #endregion

        /// <summary>
        /// MongoDB'den kayıtlı bir dokümanı ve chat geçmişini yükler (geçmiş görüntüleme)
        /// </summary>
        public async Task LoadFromMongoAsync(string documentId)
        {
            try
            {
                if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
                {
                    _dialogService.ShowError("Giriş yapılmamış.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(documentId))
                    return;

                IsProcessing = true;
                ChatMessages.Clear();

                var doc = await _pdfRepository.GetByIdAsync(_userContext.UserId!, documentId);
                if (doc == null)
                {
                    _dialogService.ShowError("Döküman bulunamadı.");
                    return;
                }

                CurrentDocument = new DocumentSummary
                {
                    DocumentId = doc.Id,
                    FileName = doc.FileName,
                    Summary = doc.Summary,
                    ModelsUsed = doc.ModelsUsed,
                    UploadDate = doc.UploadDateUtc.ToLocalTime(),
                    FileSize = doc.FileSize,
                    PageCount = doc.PageCount
                };

                _currentDocumentId = doc.Id;
                PdfText = string.Empty; // bu modda tam metin yok
                HasDocument = true;

                var msgs = await _chatRepository.GetMessagesAsync(_userContext.UserId!, doc.Id!, limit: 500);
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
                _dialogService.ShowError($"Geçmiş yükleme hatası: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}

