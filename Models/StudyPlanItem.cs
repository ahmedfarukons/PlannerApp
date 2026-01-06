using System;
using System.Xml.Serialization;

namespace StudyPlanner.Models
{
    /// <summary>
    /// Çalışma planı öğesi - Entity class
    /// SOLID: Single Responsibility - Sadece veri tutma sorumluluğu
    /// </summary>
    [XmlRoot("StudyPlanItem")]
    public class StudyPlanItem : BaseEntity
    {
        private DateTime _date;
        private int _durationMinutes;
        private string _subject = string.Empty;
        private string _notes = string.Empty;
        private string _category = string.Empty;
        private PriorityLevel _priority;
        private bool _isCompleted;

        /// <summary>
        /// Çalışma tarihi ve saati
        /// </summary>
        [XmlElement("Date")]
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Çalışma süresi (dakika cinsinden)
        /// </summary>
        [XmlElement("DurationMinutes")]
        public int DurationMinutes
        {
            get => _durationMinutes;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Süre negatif olamaz", nameof(value));
                
                _durationMinutes = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationDisplay));
                ModifiedDate = DateTime.Now;
            }
        }

        private int _actualDurationMinutes;

        /// <summary>
        /// Gerçekleşen çalışma süresi (dakika)
        /// </summary>
        [XmlElement("ActualDurationMinutes")]
        public int ActualDurationMinutes
        {
            get => _actualDurationMinutes;
            set
            {
                if (value < 0) value = 0;
                _actualDurationMinutes = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ActualDurationDisplay));
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Ders veya konu adı
        /// </summary>
        [XmlElement("Subject")]
        public string Subject
        {
            get => _subject;
            set
            {
                _subject = value ?? string.Empty;
                OnPropertyChanged();
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Çalışma notları
        /// </summary>
        [XmlElement("Notes")]
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value ?? string.Empty;
                OnPropertyChanged();
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Çalışma kategorisi (örn: Matematik, Fizik, vb.)
        /// </summary>
        [XmlElement("Category")]
        public string Category
        {
            get => _category;
            set
            {
                _category = value ?? string.Empty;
                OnPropertyChanged();
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Çalışma öncelik seviyesi
        /// </summary>
        [XmlElement("Priority")]
        public PriorityLevel Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PriorityIndex));
                OnPropertyChanged(nameof(PriorityDisplay));
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// ComboBox için index
        /// </summary>
        [XmlIgnore]
        public int PriorityIndex
        {
            get => (int)Priority;
            set
            {
                Priority = (PriorityLevel)value;
            }
        }

        /// <summary>
        /// Öncelik gösterimi (Türkçe)
        /// </summary>
        [XmlIgnore]
        public string PriorityDisplay => Priority switch
        {
            PriorityLevel.Low => "Düşük",
            PriorityLevel.Medium => "Orta",
            PriorityLevel.High => "Yüksek",
            PriorityLevel.Critical => "Kritik",
            _ => "Orta"
        };

        /// <summary>
        /// Çalışmanın tamamlanma durumu
        /// </summary>
        [XmlElement("IsCompleted")]
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged();
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Süre gösterimi (saat:dakika formatında)
        /// </summary>
        [XmlIgnore]
        public string DurationDisplay => $"{DurationMinutes / 60}:{DurationMinutes % 60:00} saat";

        /// <summary>
        /// Gerçekleşen süre gösterimi
        /// </summary>
        [XmlIgnore]
        public string ActualDurationDisplay => $"{ActualDurationMinutes / 60}:{ActualDurationMinutes % 60:00} saat";

        /// <summary>
        /// Tarih gösterimi (kısa format)
        /// </summary>
        [XmlIgnore]
        public string DateDisplay => Date.ToString("dd.MM.yyyy HH:mm");

        /// <summary>
        /// Default constructor
        /// </summary>
        public StudyPlanItem() : base()
        {
            Date = DateTime.Now;
            DurationMinutes = 30;
            Priority = PriorityLevel.Medium;
            IsCompleted = false;
        }

        /// <summary>
        /// ToString override - debug için kullanışlı
        /// </summary>
        public override string ToString()
        {
            return $"{Subject} - {DateDisplay} ({DurationDisplay})";
        }
    }

    /// <summary>
    /// Öncelik seviyeleri enum
    /// </summary>
    public enum PriorityLevel
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }
}



