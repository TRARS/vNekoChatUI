using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace vNekoChatUI.A.CustomControlEx.PromptBoxEx
{
    public class cPromptBox_converter_bordercontentheader_visibility : IValueConverter
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

    public class cPromptBox_converter_placeholder_visibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string text && values[1] is bool onoff)
            {
                if (onoff)
                {
                    return string.IsNullOrEmpty(text) ? Visibility.Visible : Visibility.Collapsed;
                }
                return Visibility.Collapsed;

            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
