using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OhKeyCapsTester.Core
{
    public class KeySizeConverter : IValueConverter
    {
        public int SizeOfKey { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (int.TryParse(value.ToString(), out var result))
            {
                return result * SizeOfKey;
            }

            return SizeOfKey;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class KeyStateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Colors.Transparent);
            return (int)value > 0 ?
                new SolidColorBrush(Colors.DarkGreen) :
                new SolidColorBrush(Colors.LightGreen);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
