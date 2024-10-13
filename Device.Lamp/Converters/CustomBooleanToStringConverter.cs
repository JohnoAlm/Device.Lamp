using System.Globalization;
using System.Windows.Data;

namespace Device.Lamp.Converters
{
    public class CustomBooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool isConfigured)
            {
                return isConfigured ? "Device Configured" : "Device Not Configured";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
