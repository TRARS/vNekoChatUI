using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace vNekoChatUI.CustomControlEx.ConsoleListViewEx
{
    public class cConsoleListView : ListView
    {
        static cConsoleListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cConsoleListView), new FrameworkPropertyMetadata(typeof(cConsoleListView)));
        }
        public cConsoleListView()
        {
            this.Foreground = new SolidColorBrush(Colors.White);
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFABADB3"));
        }
    }
}
