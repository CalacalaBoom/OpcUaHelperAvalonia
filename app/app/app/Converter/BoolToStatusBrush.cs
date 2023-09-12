using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.Converter
{
    public class BoolToStatusBrush : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null && bool.Parse(value.ToString()))
            {
                return new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                return new SolidColorBrush(Colors.LightGray);
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Colors.LightGray);
        }
    }
}
