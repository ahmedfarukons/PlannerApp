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

        private DocumentSummary _currentDocument;
        private string _pdfText;
        private string _questionText;
        private bool _isProcessing;
        private bool _hasDocument;
        private ObservableCollection<ChatMessage> _chatMessages;

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
            IDialogService dialogService)
        {
            _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            ChatMessages = new ObservableCollection<ChatMessage>();

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
                ChatMessages.Add(new ChatMessage
                {
                    IsUser = true,
                    Content = QuestionText,
                    Timestamp = DateTime.Now
                });

                var question = QuestionText;
                QuestionText = string.Empty;

                // AI'dan cevap al
                var answer = await _aiService.AskQuestionAsync(question, PdfText);

                // AI cevabını ekle
                ChatMessages.Add(new ChatMessage
                {
                    IsUser = false,
                    Content = answer,
                    Timestamp = DateTime.Now
                });
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
            CurrentDocument = null;
            PdfText = null;
            QuestionText = string.Empty;
            HasDocument = false;
            ChatMessages.Clear();
        }

        #endregion

        #region Can Execute Methods

        private bool CanAskQuestion()
        {
            return HasDocument && !IsProcessing && !string.IsNullOrWhiteSpace(QuestionText);
        }

        #endregion
    }
}

