using System.Globalization;

namespace RecipeApp.Converters
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string selectedValue && parameter is string parameterValue)
            {
                return selectedValue == parameterValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter is string parameterValue)
            {
                return parameterValue;
            }
            return null;
        }
    }
} 