using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using vNekoChatUI.A.MVVM.Enums;

namespace vNekoChatUI.A.MVVM.Converters
{
    public class NekoChat_converter_chatmode_isenabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ChatMode)value is not ChatMode.Debug);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NekoChat_converter_contextmenu_show_if_empty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace($"{value}") ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NekoChat_converter_selectedcontact_title : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var username = values[0] as string;
            var messagecount = values[1];//(values[1] as IList)?.Count;

            if (username is not null && messagecount is not null)
            {
                var flag = int.TryParse($"{messagecount}", out int msgcount);
                if (flag && msgcount > 0)
                {
                    return $"{values[0]}" + $"({msgcount})";
                }
                return $"{values[0]}";
            }

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
