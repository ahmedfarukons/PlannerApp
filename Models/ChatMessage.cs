using System;

namespace StudyPlanner.Models
{
    /// <summary>
    /// Sohbet mesajı modeli
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Mesajın kullanıcıdan mı yoksa AI'dan mı geldiği
        /// </summary>
        public bool IsUser { get; set; }

        /// <summary>
        /// Mesaj içeriği
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Mesaj zamanı
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Mesaj gösterimi
        /// </summary>
        public string TimeDisplay => Timestamp.ToString("HH:mm");
    }
}

