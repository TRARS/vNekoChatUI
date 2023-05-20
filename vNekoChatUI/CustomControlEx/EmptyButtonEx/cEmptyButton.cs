using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace vNekoChatUI.CustomControlEx.EmptyButtonEx
{
    public partial class cEmptyButton : Button
    {
        static cEmptyButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cEmptyButton), new FrameworkPropertyMetadata(typeof(cEmptyButton)));
        }
    }
    public partial class cEmptyButton
    {
        public string BorderMouseOverColor
        {
            get { return (string)GetValue(BorderMouseOverColorProperty); }
            set { SetValue(BorderMouseOverColorProperty, value); }
        }
        public static readonly DependencyProperty BorderMouseOverColorProperty = DependencyProperty.Register(
            name: "BorderMouseOverColor",
            propertyType: typeof(string),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata($"{Colors.DarkRed}", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public string BorderPathColor
        {
            get { return (string)GetValue(BorderPathColorProperty); }
            set { SetValue(BorderPathColorProperty, value); }
        }
        public static readonly DependencyProperty BorderPathColorProperty = DependencyProperty.Register(
            name: "BorderPathColor",
            propertyType: typeof(string),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata($"#5f606f", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public string BorderPathData
        {
            get { return (string)GetValue(BorderPathDataProperty); }
            set { SetValue(BorderPathDataProperty, value); }
        }
        public static readonly DependencyProperty BorderPathDataProperty = DependencyProperty.Register(
            name: "BorderPathData",
            propertyType: typeof(string),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public double BorderBackgroundOpacity
        {
            get { return (double)GetValue(BorderBackgroundOpacityProperty); }
            set { SetValue(BorderBackgroundOpacityProperty, value); }
        }
        public static readonly DependencyProperty BorderBackgroundOpacityProperty = DependencyProperty.Register(
            name: "BorderBackgroundOpacity",
            propertyType: typeof(double),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public Thickness BorderPathMargin
        {
            get { return (Thickness)GetValue(BorderPathMarginProperty); }
            set { SetValue(BorderPathMarginProperty, value); }
        }
        public static readonly DependencyProperty BorderPathMarginProperty = DependencyProperty.Register(
            name: "BorderPathMargin",
            propertyType: typeof(Thickness),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(new Thickness(4), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );




        public bool DisableContextMenu
        {
            get { return (bool)GetValue(DisableContextMenuProperty); }
            set { SetValue(DisableContextMenuProperty, value); }
        }
        public static readonly DependencyProperty DisableContextMenuProperty = DependencyProperty.Register(
            name: "DisableContextMenu",
            propertyType: typeof(bool),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public ObservableCollection<string> ChatGptApiKeys
        {
            get { return (ObservableCollection<string>)GetValue(ChatGptApiKeysProperty); }
            set { SetValue(ChatGptApiKeysProperty, value); }
        }
        public static readonly DependencyProperty ChatGptApiKeysProperty = DependencyProperty.Register(
            name: "ChatGptApiKeys",
            propertyType: typeof(ObservableCollection<string>),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
