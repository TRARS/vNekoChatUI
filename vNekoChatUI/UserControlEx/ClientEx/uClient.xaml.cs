using Common;
using System.Windows.Controls;
using vNekoChatUI.Extensions;

namespace vNekoChatUI.UserControlEx.ClientEx
{
    /// <summary>
    /// uClient.xaml 的交互逻辑
    /// </summary>
    public partial class uClient : UserControl
    {
        public uClient()
        {
            InitializeComponent();
            this.DataContext = new uClient_viewmodel();

        }

        private void sourceText_TextChanged(object sender, TextChangedEventArgs e)
        {
            object? temp = MarkdownViewer1?.Document?.GetFormattedText().WidthIncludingTrailingWhitespace;
            LogProxy.Instance.Print($"{temp}");
        }
    }
}
