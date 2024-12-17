using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace vNekoChatUI.A.CustomControlEx.ListViewChatHistoryEx
{
    //修正
    public class cScrollViewer_converter_iscapturing_border_mask_visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
