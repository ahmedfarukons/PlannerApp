using System;
using System.Globalization;
using System.Windows.Data;
using StudyPlanner.Models;

namespace StudyPlanner.Helpers
{
    /// <summary>
    /// Priority enum değerlerini Türkçe metne çevirir
    /// </summary>
    public class PriorityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PriorityLevel priority)
            {
                return priority switch
                {
                    PriorityLevel.Low => "Düşük",
                    PriorityLevel.Medium => "Orta",
                    PriorityLevel.High => "Yüksek",
                    PriorityLevel.Critical => "Kritik",
                    _ => value.ToString()
                };
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return text switch
                {
                    "Düşük" => PriorityLevel.Low,
                    "Orta" => PriorityLevel.Medium,
                    "Yüksek" => PriorityLevel.High,
                    "Kritik" => PriorityLevel.Critical,
                    _ => PriorityLevel.Medium
                };
            }
            return PriorityLevel.Medium;
        }
    }
}


