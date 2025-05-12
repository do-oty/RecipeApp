using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace RecipeApp.Converters
{
    public class TruncateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && parameter != null && int.TryParse(parameter.ToString(), out int maxLength))
            {
                if (str.Length > maxLength)
                    return str.Substring(0, maxLength) + "...";
                return str;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 