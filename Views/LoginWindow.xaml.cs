using System.Windows;
using StudyPlanner.ViewModels;

namespace StudyPlanner.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.AuthCompleted += success =>
            {
                if (success)
                {
                    Dispatcher.Invoke(() =>
                    {
                        DialogResult = true;
                        Close();
                    });
                }
            };
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                // Tab 0: login, Tab 1: register
                if (vm.SelectedTabIndex == 0)
                    vm.LoginPassword = PasswordBox.Password;
                else
                    vm.RegisterPassword = PasswordBoxRegister.Password;
            }
        }
    }
}


