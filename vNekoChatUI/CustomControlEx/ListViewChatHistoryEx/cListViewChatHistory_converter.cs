using ColorHelper;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace vNekoChatUI.CustomControlEx.ListViewChatHistoryEx
{
    //
    public class cListViewChatHistory_converter_textblock_maxwidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - 40 - 100;//40是非气泡的列的宽度
                                            //100是让气泡不要太靠边
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class cListViewChatHistory_converter_contactcard_time_token : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return values.Aggregate((x, y) => string.Format($"{((DateTime)x).ToString("H:mm:ss tt", new CultureInfo("ja-JP"))}, (total_tokens: {y})"));
            }
            catch
            {
                return "time_token_error";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class cListViewChatHistory_converter_borderbrush : IValueConverter
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

    public class cListViewChatHistory_converter_imagesource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imagePath = $"{value}";
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                try
                {
                    if (Path.IsPathRooted(imagePath))
                    {
                        // 尝试将路径转换为 ImageSource 对象
                        var imageSource = new BitmapImage(new Uri(imagePath));
                        return imageSource;
                    }
                    else
                    {
                        // 尝试将路径转换为 ImageSource 对象
                        string packUri = $"pack://application:,,,/{imagePath}";
                        // 尝试将 Pack URI 转换为 ImageSource 对象
                        var imageSource = new BitmapImage(new Uri(packUri));
                        return imageSource;
                    }
                }
                catch { }
            }

            return new WriteableBitmap(32, 32, 96, 96, PixelFormats.Bgra32, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class cListViewChatHistory_converter_contextmenu_hide_if_empty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace($"{value}") ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class cListViewChatHistory_converter_contextmenu_show_if_empty : IValueConverter
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

    public class cListViewChatHistory_converter_menuitem_commandparameter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Tuple.Create(values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
