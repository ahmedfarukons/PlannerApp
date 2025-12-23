using System;
using System.Threading.Tasks;
using System.Windows.Input;
using StudyPlanner.Interfaces;
using StudyPlanner.Services;

namespace StudyPlanner.ViewModels
{
    /// <summary>
    /// Login/Register ekranı ViewModel'i (email/username ile giriş, email+adsoyad ile kayıt).
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly AuthCredentialStore _credentialStore;

        private string _loginIdentifier = string.Empty;
        private string _loginPassword = string.Empty;

        private string _registerUsername = string.Empty;
        private string _registerEmail = string.Empty;
        private string _registerFullName = string.Empty;
        private string _registerPassword = string.Empty;

        private int _selectedTabIndex; // 0: login, 1: register
        private string _statusMessage = string.Empty;
        private bool _isBusy;
        private bool _rememberMe;

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public string LoginIdentifier
        {
            get => _loginIdentifier;
            set => SetProperty(ref _loginIdentifier, value);
        }

        public string LoginPassword
        {
            get => _loginPassword;
            set => SetProperty(ref _loginPassword, value);
        }

        public string RegisterUsername
        {
            get => _registerUsername;
            set => SetProperty(ref _registerUsername, value);
        }

        public string RegisterEmail
        {
            get => _registerEmail;
            set => SetProperty(ref _registerEmail, value);
        }

        public string RegisterFullName
        {
            get => _registerFullName;
            set => SetProperty(ref _registerFullName, value);
        }

        public string RegisterPassword
        {
            get => _registerPassword;
            set => SetProperty(ref _registerPassword, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public event Action<bool>? AuthCompleted; // true => success

        public LoginViewModel(IUserService userService, AuthCredentialStore credentialStore)
        {
            _userService = userService;
            _credentialStore = credentialStore;
            StatusMessage = "Hazır.";
            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => !IsBusy);
            RegisterCommand = new RelayCommand(async _ => await RegisterAsync(), _ => !IsBusy);
        }

        private async Task LoginAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Giriş yapılıyor...";
                await Task.Yield(); // let UI paint status/loading immediately

                var result = await _userService.LoginAsync(LoginIdentifier, LoginPassword);
                if (!result.Success)
                {
                    StatusMessage = result.ErrorMessage;
                    AuthCompleted?.Invoke(false);
                    return;
                }

                StatusMessage = "Başarılı!";
                if (RememberMe)
                    _credentialStore.Save(LoginIdentifier, LoginPassword);
                else
                    _credentialStore.Clear();
                AuthCompleted?.Invoke(true);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Hata: {ex.Message}";
                AuthCompleted?.Invoke(false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RegisterAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Kayıt oluşturuluyor...";
                await Task.Yield(); // let UI paint status/loading immediately

                var result = await _userService.RegisterAsync(RegisterUsername, RegisterEmail, RegisterFullName, RegisterPassword);
                if (!result.Success)
                {
                    StatusMessage = result.ErrorMessage;
                    AuthCompleted?.Invoke(false);
                    return;
                }

                StatusMessage = "Kayıt başarılı!";
                // Register sonrası giriş için email tercih edelim
                if (RememberMe)
                    _credentialStore.Save(RegisterEmail, RegisterPassword);
                else
                    _credentialStore.Clear();
                AuthCompleted?.Invoke(true);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Hata: {ex.Message}";
                AuthCompleted?.Invoke(false);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}


