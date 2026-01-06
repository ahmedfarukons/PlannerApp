using System;
using System.Threading.Tasks;
using System.Windows.Input;
using StudyPlanner.Interfaces;
using StudyPlanner.Models;

namespace StudyPlanner.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IUserContext _userContext;
        private readonly IDialogService _dialogService;
        private readonly IUiSettingsService _uiSettingsService;

        private string _username;
        private string _fullName;
        private string _email;
        private string _currentPassword;
        private string _newPassword;

        public ProfileViewModel(
            IUserService userService,
            IUserContext userContext,
            IDialogService dialogService,
            IUiSettingsService uiSettingsService)
        {
            _userService = userService;
            _userContext = userContext;
            _dialogService = dialogService;
            _uiSettingsService = uiSettingsService;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            ChangePasswordCommand = new RelayCommand(async _ => await ChangePasswordAsync());
            
            LoadUserData();
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string CurrentPassword
        {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public double UiScale
        {
            get => _uiSettingsService.UiScale;
            set
            {
                if (_uiSettingsService.UiScale != value)
                {
                    _uiSettingsService.UiScale = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        private async void LoadUserData()
        {
            if (!_userContext.IsAuthenticated) return;

            try
            {
                var user = await _userService.GetUserAsync(_userContext.UserId);
                if (user != null)
                {
                    Username = user.Username;
                    FullName = user.FullName;
                    Email = user.Email;
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Kullanıcı bilgileri yüklenemedi: {ex.Message}");
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                var user = await _userService.GetUserAsync(_userContext.UserId);
                if (user != null)
                {
                    user.FullName = FullName;
                    user.Email = Email;
                    // Username usually not changeable or requires check
                    
                    await _userService.UpdateUserAsync(user);
                    _dialogService.ShowMessage("Profil güncellendi!");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Güncelleme hatası: {ex.Message}");
            }
        }

        private async Task ChangePasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
            {
                _dialogService.ShowError("Mevcut ve yeni şifre gereklidir.");
                return;
            }

            try
            {
                var success = await _userService.ChangePasswordAsync(_userContext.UserId, CurrentPassword, NewPassword);
                if (success)
                {
                    _dialogService.ShowMessage("Şifre başarıyla değiştirildi!");
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                }
                else
                {
                    _dialogService.ShowError("Şifre değiştirilemedi. Mevcut şifrenizi kontrol edin.");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Şifre değiştirme hatası: {ex.Message}");
            }
        }
    }
}
