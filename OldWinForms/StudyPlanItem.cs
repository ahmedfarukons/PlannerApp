using System;

namespace StudyPlanner
{
    /// <summary>
    /// Old WinForms prototipi için çalışma planı item modeli.
    /// (Bu klasör compile dışında, sadece arşiv amaçlı tutulur.)
    /// </summary>
    public class StudyPlanItem
    {
        public DateTime Date { get; set; }
        public int DurationMinutes { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}


