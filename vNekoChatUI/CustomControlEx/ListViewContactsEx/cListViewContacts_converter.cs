using ColorHelper;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace vNekoChatUI.CustomControlEx.ListViewContactsEx
{
    public class cListViewContacts_converter_borderbrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color;//= (Color)System.Windows.Media.ColorConverter.ConvertFromString($"{value}");

            try
            {
                color = (Color)System.Windows.Media.ColorConverter.ConvertFromString($"{value}");
            }
            catch
            {
                if (parameter is not null && parameter.ToString() == "getColorStr")
                {
                    return $"#000000";
                }
                else
                {
                    var color_new = (Color)System.Windows.Media.ColorConverter.ConvertFromString($"#FF0000");
                    return color_new;
                }
            }

            HSV hsv = ColorHelper.ColorConverter.RgbToHsv(new RGB(color.R, color.G, color.B));
            hsv.H += 180;
            hsv.S = (byte)(hsv.S / 4 + 10);
            //hsv.V *=2;
            RGB rgb = ColorHelper.ColorConverter.HsvToRgb(hsv);

            if (parameter is not null && parameter.ToString() == "getColorStr")
            {
                return $"#{color.A:x2}{rgb.R:x2}{rgb.G:x2}{rgb.B:x2}";
            }
            else
            {
                var color_new = (Color)System.Windows.Media.ColorConverter.ConvertFromString($"#{color.A:x2}{rgb.R:x2}{rgb.G:x2}{rgb.B:x2}");
                //LogProxy.Instance.Print($"{color} -> {color_new} -> {parameter}");
                return color_new;
            }
            //SolidColorBrush.Color可以接收 Color, 也可以接收 字符串
            //Foreground只能接收 字符串
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
