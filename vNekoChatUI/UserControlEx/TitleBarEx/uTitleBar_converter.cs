using System;
using System.Globalization;
using System.Windows.Data;

namespace vNekoChatUI.UserControlEx.TitleBarEx
{
    public class uTitleBar_converter_chatmode_isenabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value ? "Snow" : "Gray");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
