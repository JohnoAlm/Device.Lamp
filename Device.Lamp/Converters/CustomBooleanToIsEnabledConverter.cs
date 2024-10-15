using System.Globalization;
using System.Windows.Data;

namespace Device.Lamp.Converters;

public class CustomBooleanToIsEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is bool isConfigured)
        {
            return isConfigured ? false : true;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
