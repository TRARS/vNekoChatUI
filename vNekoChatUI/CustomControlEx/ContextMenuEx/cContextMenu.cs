using System.Windows;
using System.Windows.Controls;
using vNekoChatUI.CustomControlEx.GridEx;

namespace vNekoChatUI.CustomControlEx.ContextMenuEx
{
    public partial class cContextMenu : ContextMenu
    {
        static cContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cContextMenu), new FrameworkPropertyMetadata(typeof(cContextMenu)));
        }
        public cContextMenu()
        {
            this.StaysOpen = true;
            this.Focusable = true;

        }
    }

    public partial class cContextMenu
    {
        public object MenuItemHeader
        {
            get { return (string)GetValue(MenuItemHeaderProperty); }
            set { SetValue(MenuItemHeaderProperty, value); }
        }
        public static readonly DependencyProperty MenuItemHeaderProperty = DependencyProperty.Register(
            name: "MenuItemHeader",
            propertyType: typeof(object),
            ownerType: typeof(cGrid),
            typeMetadata: new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
