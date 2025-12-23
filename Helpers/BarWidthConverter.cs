using System;
using System.Globalization;
using System.Windows.Data;

namespace StudyPlanner.Helpers
{
    /// <summary>
    /// Saat değerini bar genişliğine çeviren converter
    /// </summary>
    public class BarWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double hours)
            {
                // Her saat için 30 pixel (max 12 saat = 360px)
                return Math.Min(hours * 30, 360);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}




