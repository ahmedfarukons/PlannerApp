using System;
using System.Globalization;
using System.Windows.Data;

namespace StudyPlanner.Helpers
{
    /// <summary>
    /// True -> False, False -> True
    /// XAML'de IsEnabled gibi boolean binding'leri ters çevirmek için kullanılır.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;

            return false;
        }
    }
}





