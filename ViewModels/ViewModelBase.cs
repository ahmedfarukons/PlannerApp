using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudyPlanner.ViewModels
{
    /// <summary>
    /// Tüm ViewModeller için base class
    /// INotifyPropertyChanged implementasyonu ile data binding desteği
    /// SOLID: Open/Closed Principle - Genişletmeye açık, değişikliğe kapalı
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// PropertyChanged event - WPF binding için gerekli
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property değişikliklerini bildirir
        /// CallerMemberName attribute sayesinde property adı otomatik alınır
        /// </summary>
        /// <param name="propertyName">Değişen property adı</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property değerini set eder ve değişiklik varsa bildirir
        /// Kod tekrarını önler
        /// </summary>
        /// <typeparam name="T">Property tipi</typeparam>
        /// <param name="field">Backing field</param>
        /// <param name="value">Yeni değer</param>
        /// <param name="propertyName">Property adı</param>
        /// <returns>Değer değiştiyse true</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}



