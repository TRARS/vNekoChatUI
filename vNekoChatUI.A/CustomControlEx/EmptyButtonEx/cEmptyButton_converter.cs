using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace vNekoChatUI.A.CustomControlEx.EmptyButtonEx
{
    //圆角裁剪
    public class cEmptyButton_converter_border_clip : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var width = (double)values[0];
            var height = (double)values[1];
            var thickness = (Thickness)values[2];
            var minus_x = thickness.Left + thickness.Right;
            var minus_y = thickness.Top + thickness.Bottom;
            return new Rect(0, 0, width, height);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //修正
    public class cEmptyButton_converter_border_mask_margin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var thickness = (Thickness)value;
            return new Thickness(-thickness.Left, -thickness.Top, -thickness.Right, -thickness.Bottom);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
