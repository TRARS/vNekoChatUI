using System.Windows;
using System.Windows.Controls;
using vNekoChatUI.Base.Helper;

namespace vNekoChatUI.UserControlEx.TitleBarEx
{
    /// <summary>
    /// uTitleBar.xaml 的交互逻辑
    /// </summary>
    public partial class uTitleBar : UserControl
    {
        public uTitleBar()
        {
            InitializeComponent();
            this.DataContext = new uTitleBar_viewmodel();
        }
    }

    public partial class uTitleBar
    {
        #region 缺省按钮的
        private void ResetPosButton_Click(object sender, RoutedEventArgs e)
        {
            Mediator.Instance.NotifyColleagues(MessageType.WindowPosReset, new Vector(0, 0));
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Mediator.Instance.NotifyColleagues(MessageType.WindowMinimize, null);
        }
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            Mediator.Instance.NotifyColleagues(MessageType.WindowMaximize, null);
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Mediator.Instance.NotifyColleagues(MessageType.WindowClose, null);
        }
        #endregion
    }

    public partial class uTitleBar
    {
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            name: "IsActive",
            propertyType: typeof(bool),
            ownerType: typeof(uTitleBar),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
