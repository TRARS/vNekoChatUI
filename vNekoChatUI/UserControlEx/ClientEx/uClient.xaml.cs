using System.Windows;
using System.Windows.Controls;

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

        //private void sourceText_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    object? temp = MarkdownViewer1?.Document?.GetFormattedText().WidthIncludingTrailingWhitespace;
        //    LogProxy.Instance.Print($"{temp}");
        //}
    }

    public partial class uClient
    {
        public bool ShowResizer
        {
            get { return (bool)GetValue(ShowResizerProperty); }
            set { SetValue(ShowResizerProperty, value); }
        }
        public static readonly DependencyProperty ShowResizerProperty = DependencyProperty.Register(
            name: "ShowResizer",
            propertyType: typeof(bool),
            ownerType: typeof(uClient),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
