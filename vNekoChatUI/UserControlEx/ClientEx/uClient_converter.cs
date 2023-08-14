using Common.WPF;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using vNekoChatUI.Extensions;

namespace vNekoChatUI.UserControlEx.ClientEx
{
    public class uClient_converter_chatmode_int : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }

    public class uClient_converter_chatmode_isenabled : IValueConverter
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

    public class uClient_converter_textchanged : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //System.Windows.Documents.FlowDocument
            var temp = value as System.Windows.Documents.FlowDocument;
            double width = temp!.GetFormattedText().WidthIncludingTrailingWhitespace;
            double width_fix = width + width switch
            {
                > 160 => 20,
                > 80 => 40,
                > 40 => 80,
                > 20 => 160,
                _ => 0,
            };
            LogProxy.Instance.Print($"{width} -> {width_fix}");
            return width;//
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class uClient_converter_borderbrush : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        Color color;//= (Color)System.Windows.Media.ColorConverter.ConvertFromString($"{value}");

    //        try
    //        {
    //            color = (Color)System.Windows.Media.ColorConverter.ConvertFromString($"{value}");
    //        }
    //        catch
    //        {
    //            if (parameter is not null && parameter.ToString() == "getColorStr")
    //            {
    //                return $"#000000";
    //            }
    //            else
    //            {
    //                var color_new = (Color)System.Windows.Media.ColorConverter.ConvertFromString($"#FF0000");
    //                return color_new;
    //            }
    //        }

    //        HSV hsv = ColorHelper.ColorConverter.RgbToHsv(new RGB(color.R, color.G, color.B));
    //        hsv.H += 180;
    //        hsv.S = (byte)(hsv.S / 4 + 10);
    //        //hsv.V *=2;
    //        RGB rgb = ColorHelper.ColorConverter.HsvToRgb(hsv);

    //        if (parameter is not null && parameter.ToString() == "getColorStr")
    //        {
    //            return $"#{color.A:x2}{rgb.R:x2}{rgb.G:x2}{rgb.B:x2}";
    //        }
    //        else
    //        {
    //            var color_new = (Color)System.Windows.Media.ColorConverter.ConvertFromString($"#{color.A:x2}{rgb.R:x2}{rgb.G:x2}{rgb.B:x2}");
    //            //LogProxy.Instance.Print($"{color} -> {color_new} -> {parameter}");
    //            return color_new;
    //        }
    //        //SolidColorBrush.Color可以接收 Color, 也可以接收 字符串
    //        //Foreground只能接收 字符串
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class uClient_converter_contextmenu_show_if_empty : IValueConverter
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



    public class uClient_converter_selectedcontact_title : IMultiValueConverter
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
