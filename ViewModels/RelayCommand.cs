using System;
using System.Windows.Input;

namespace StudyPlanner.ViewModels
{
    /// <summary>
    /// ICommand implementasyonu - MVVM pattern için
    /// Command pattern kullanarak view-viewmodel bağlantısı sağlar
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Constructor - CanExecute kontrolü olmadan
        /// </summary>
        /// <param name="execute">Çalıştırılacak aksiyon</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Constructor - CanExecute kontrolü ile
        /// </summary>
        /// <param name="execute">Çalıştırılacak aksiyon</param>
        /// <param name="canExecute">Çalıştırılabilirlik kontrolü</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// CanExecute değişikliğini bildirir
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Komutun çalıştırılabilir olup olmadığını kontrol eder
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Komutu çalıştırır
        /// </summary>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}



