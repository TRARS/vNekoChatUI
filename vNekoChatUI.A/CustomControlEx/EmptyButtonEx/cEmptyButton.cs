using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace vNekoChatUI.A.CustomControlEx.EmptyButtonEx
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

        public string BorderBackgroundZeroColor
        {
            get { return (string)GetValue(BorderBackgroundZeroColorProperty); }
            set { SetValue(BorderBackgroundZeroColorProperty, value); }
        }
        public static readonly DependencyProperty BorderBackgroundZeroColorProperty = DependencyProperty.Register(
            name: "BorderBackgroundZeroColor",
            propertyType: typeof(string),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata("#00FFFFFF", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
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

        //?
        public bool LightOnOff
        {
            get { return (bool)GetValue(LightOnOffProperty); }
            set { SetValue(LightOnOffProperty, value); }
        }
        public static readonly DependencyProperty LightOnOffProperty = DependencyProperty.Register(
            name: "LightOnOff",
            propertyType: typeof(bool),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        //供外部操作内部ContextMenu开关
        public bool ContextMenuIsOpen
        {
            get { return (bool)GetValue(ContextMenuIsOpenProperty); }
            set { SetValue(ContextMenuIsOpenProperty, value); }
        }
        public static readonly DependencyProperty ContextMenuIsOpenProperty = DependencyProperty.Register(
            name: "ContextMenuIsOpen",
            propertyType: typeof(bool),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
        //是否允许展开ContextMenu
        public bool ContextMenuIsEnabled
        {
            get { return (bool)GetValue(ContextMenuIsEnabledProperty); }
            set { SetValue(ContextMenuIsEnabledProperty, value); }
        }
        public static readonly DependencyProperty ContextMenuIsEnabledProperty = DependencyProperty.Register(
            name: "ContextMenuIsEnabled",
            propertyType: typeof(bool),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
        //ContextMenu展开位置
        public PlacementMode ContextMenuPlacementMode
        {
            get { return (PlacementMode)GetValue(ContextMenuPlacementModeProperty); }
            set { SetValue(ContextMenuPlacementModeProperty, value); }
        }
        public static readonly DependencyProperty ContextMenuPlacementModeProperty = DependencyProperty.Register(
            name: "ContextMenuPlacementMode",
            propertyType: typeof(PlacementMode),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(PlacementMode.MousePoint, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
        //ContextMenu内容
        public UIElement ContextMenuContent
        {
            get { return (UIElement)GetValue(ContextMenuContentProperty); }
            set { SetValue(ContextMenuContentProperty, value); }
        }
        public static readonly DependencyProperty ContextMenuContentProperty = DependencyProperty.Register(
            name: "ContextMenuContent",
            propertyType: typeof(UIElement),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register(
            name: "ButtonText",
            propertyType: typeof(string),
            ownerType: typeof(cEmptyButton),
            typeMetadata: new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
