using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CheckListToolWPF.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val;
            bool result = bool.TryParse(value.ToString(), out val);
            if (!result) return val;
            if(val && Negate)
            {
                return Visibility.Collapsed;
            }
            else if (!val && Negate)
            {
                return Visibility.Visible;
            }
            else if (!val && !Negate)
            {
                return Visibility.Collapsed;
            }
            else if (val && !Negate)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public bool Negate { set; get; }
    }
}
