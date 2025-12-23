using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace StudyPlanner.Helpers
{
    /// <summary>
    /// Enum tipini koleksiyona çeviren converter
    /// WPF ComboBox için kullanılır
    /// </summary>
    public class EnumToCollectionConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EnumToCollectionConverter Instance { get; } = new EnumToCollectionConverter();

        /// <summary>
        /// Enum'u koleksiyona çevirir
        /// </summary>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return Array.Empty<object>();

            return Enum.GetValues(value as Type ?? value.GetType()).Cast<object>();
        }

        /// <summary>
        /// Kullanılmaz
        /// </summary>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value ?? Array.Empty<object>();
        }

        /// <summary>
        /// MarkupExtension için override
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}



