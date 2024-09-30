using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Device.Lamp.Converters;

public class CustomBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is bool boolValue)
        {
            if(parameter is string param)
            {
                if(param == "Reverse")
                    return !boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility visibility && visibility == Visibility.Visible;
    }
}
