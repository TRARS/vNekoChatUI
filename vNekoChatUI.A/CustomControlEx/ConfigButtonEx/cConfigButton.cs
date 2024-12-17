using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace vNekoChatUI.A.CustomControlEx.ConfigButtonEx
{
    public partial class cConfigButton : Button
    {
        static cConfigButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cConfigButton), new FrameworkPropertyMetadata(typeof(cConfigButton)));
        }
    }

    //外观
    public partial class cConfigButton
    {
        public string BorderMouseOverColor
        {
            get { return (string)GetValue(BorderMouseOverColorProperty); }
            set { SetValue(BorderMouseOverColorProperty, value); }
        }
        public static readonly DependencyProperty BorderMouseOverColorProperty = DependencyProperty.Register(
            name: "BorderMouseOverColor",
            propertyType: typeof(string),
            ownerType: typeof(cConfigButton),
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
            ownerType: typeof(cConfigButton),
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
            ownerType: typeof(cConfigButton),
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
            ownerType: typeof(cConfigButton),
            typeMetadata: new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );


        public bool DisableContextMenu
        {
            get { return (bool)GetValue(DisableContextMenuProperty); }
            set { SetValue(DisableContextMenuProperty, value); }
        }
        public static readonly DependencyProperty DisableContextMenuProperty = DependencyProperty.Register(
            name: "DisableContextMenu",
            propertyType: typeof(bool),
            ownerType: typeof(cConfigButton),
            typeMetadata: new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );


    }

    //样式内的ContextMenu内使用
    public partial class cConfigButton
    {
        public object ChatGptApiKeys
        {
            get { return (object)GetValue(ChatGptApiKeysProperty); }
            set { SetValue(ChatGptApiKeysProperty, value); }
        }
        public static readonly DependencyProperty ChatGptApiKeysProperty = DependencyProperty.Register(
            name: "ChatGptApiKeys",
            propertyType: typeof(object),
            ownerType: typeof(cConfigButton),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public object BingGptCookies
        {
            get { return (object)GetValue(BingGptCookiesProperty); }
            set { SetValue(BingGptCookiesProperty, value); }
        }
        public static readonly DependencyProperty BingGptCookiesProperty = DependencyProperty.Register(
            name: "BingGptCookies",
            propertyType: typeof(object),
            ownerType: typeof(cConfigButton),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public object GeminiApiKeys
        {
            get { return (object)GetValue(GeminiApiKeysProperty); }
            set { SetValue(GeminiApiKeysProperty, value); }
        }
        public static readonly DependencyProperty GeminiApiKeysProperty = DependencyProperty.Register(
            name: "GeminiApiKeys",
            propertyType: typeof(object),
            ownerType: typeof(cConfigButton),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public RelayCommand AddChatGptApiKeyCommand
        {
            get { return (RelayCommand)GetValue(AddChatGptApiKeyCommandProperty); }
            set { SetValue(AddChatGptApiKeyCommandProperty, value); }
        }
        public static readonly DependencyProperty AddChatGptApiKeyCommandProperty = DependencyProperty.Register(
            name: "AddChatGptApiKeyCommand",
            propertyType: typeof(RelayCommand),
            ownerType: typeof(cConfigButton),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public RelayCommand AddBingGptCookieCommand
        {
            get { return (RelayCommand)GetValue(AddBingGptCookieCommandProperty); }
            set { SetValue(AddBingGptCookieCommandProperty, value); }
        }
        public static readonly DependencyProperty AddBingGptCookieCommandProperty = DependencyProperty.Register(
            name: "AddBingGptCookieCommand",
            propertyType: typeof(RelayCommand),
            ownerType: typeof(cConfigButton),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public RelayCommand AddGeminiApiKeyCommand
        {
            get { return (RelayCommand)GetValue(AddGeminiApiKeyCommandProperty); }
            set { SetValue(AddGeminiApiKeyCommandProperty, value); }
        }
        public static readonly DependencyProperty AddGeminiApiKeyCommandProperty = DependencyProperty.Register(
            name: "AddGeminiApiKeyCommand",
            propertyType: typeof(RelayCommand),
            ownerType: typeof(cConfigButton),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
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
            ownerType: typeof(cConfigButton),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
