using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace vNekoChatUI.CustomControlEx.TextBoxEx
{
    public class cTextBox_converter_bordercontentheader_visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return string.IsNullOrWhiteSpace(text) ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
