using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudyPlanner.Models
{
    /// <summary>
    /// Tüm entity sınıfları için base class
    /// INotifyPropertyChanged interface'ini implement eder
    /// </summary>
    public abstract class BaseEntity : INotifyPropertyChanged
    {
        private Guid _id;
        private DateTime _createdDate;
        private DateTime _modifiedDate;

        /// <summary>
        /// Entity'nin benzersiz ID'si
        /// </summary>
        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Entity'nin oluşturulma tarihi
        /// </summary>
        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                _createdDate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Entity'nin son değiştirilme tarihi
        /// </summary>
        public DateTime ModifiedDate
        {
            get => _modifiedDate;
            set
            {
                _modifiedDate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// BaseEntity constructor
        /// Yeni entity oluşturulduğunda ID ve tarih bilgilerini set eder
        /// </summary>
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Property değişikliklerini bildirir
        /// </summary>
        /// <param name="propertyName">Değişen property'nin adı</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}



