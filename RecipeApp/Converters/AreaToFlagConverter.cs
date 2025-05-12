using System.Globalization;

namespace RecipeApp.Converters
{
    public class AreaToFlagConverter : IValueConverter
    {
        private readonly Dictionary<string, string> _areaToFlag = new()
        {
            { "American", "🇺🇸" },
            { "British", "🇬🇧" },
            { "Chinese", "🇨🇳" },
            { "French", "🇫🇷" },
            { "Indian", "🇮🇳" },
            { "Italian", "🇮🇹" },
            { "Japanese", "🇯🇵" },
            { "Mexican", "🇲🇽" },
            { "Spanish", "🇪🇸" },
            { "Thai", "🇹🇭" },
            { "Turkish", "🇹🇷" },
            { "Vietnamese", "🇻🇳" },
            { "Unknown", "🌍" }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string area)
            {
                return _areaToFlag.TryGetValue(area, out string flag) ? flag : "🌍";
            }
            return "🌍";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 