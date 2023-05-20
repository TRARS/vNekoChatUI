using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace vNekoChatUI.CustomControlEx.ConsoleListBoxEx
{


    public class cConsoleListBox : ListBox
    {
        static cConsoleListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cConsoleListBox), new FrameworkPropertyMetadata(typeof(cConsoleListBox)));
        }
        public cConsoleListBox()
        {
            this.Foreground = new SolidColorBrush(Colors.White);
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ABADB3"));
            //ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
            //ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Disabled);

            this.Effect = new DropShadowEffect()
            {
                BlurRadius = 2,
                ShadowDepth = 0,
                Color = Colors.DarkRed,
                Opacity = 0.5d
            };
        }
    }


}
