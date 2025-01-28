using System.Windows;
using System.Windows.Controls;

namespace vNekoChatUI.A.CustomControlEx.PromptBoxEx
{
    public partial class cPromptBox : TextBox
    {
        static cPromptBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cPromptBox), new FrameworkPropertyMetadata(typeof(cPromptBox)));
        }
    }

    public partial class cPromptBox
    {
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            name: "Placeholder",
            propertyType: typeof(string),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata("empty", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
        public string PlaceholderColor
        {
            get { return (string)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }
        public static readonly DependencyProperty PlaceholderColorProperty = DependencyProperty.Register(
            name: "PlaceholderColor",
            propertyType: typeof(string),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata("#FF000000", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
        public bool PlaceholderOnOff
        {
            get { return (bool)GetValue(PlaceholderOnOffProperty); }
            set { SetValue(PlaceholderOnOffProperty, value); }
        }
        public static readonly DependencyProperty PlaceholderOnOffProperty = DependencyProperty.Register(
            name: "PlaceholderOnOff",
            propertyType: typeof(bool),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );


        public string BorderBackground
        {
            get { return (string)GetValue(BorderBackgroundProperty); }
            set { SetValue(BorderBackgroundProperty, value); }
        }
        public static readonly DependencyProperty BorderBackgroundProperty = DependencyProperty.Register(
            name: "BorderBackground",
            propertyType: typeof(string),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata("#00000000", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public CornerRadius BorderCornerRadius
        {
            get { return (CornerRadius)GetValue(BorderCornerRadiusProperty); }
            set { SetValue(BorderCornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty BorderCornerRadiusProperty = DependencyProperty.Register(
            name: "BorderCornerRadius",
            propertyType: typeof(CornerRadius),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(new CornerRadius(2.5), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public Thickness BorderContentMargin
        {
            get { return (Thickness)GetValue(BorderContentMarginProperty); }
            set { SetValue(BorderContentMarginProperty, value); }
        }
        public static readonly DependencyProperty BorderContentMarginProperty = DependencyProperty.Register(
            name: "BorderContentMargin",
            propertyType: typeof(Thickness),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(new Thickness(4), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public string BorderContentHeader
        {
            get { return (string)GetValue(BorderContentHeaderProperty); }
            set { SetValue(BorderContentHeaderProperty, value); }
        }
        public static readonly DependencyProperty BorderContentHeaderProperty = DependencyProperty.Register(
            name: "BorderContentHeader",
            propertyType: typeof(string),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
        public string BorderContentHeaderColor
        {
            get { return (string)GetValue(BorderContentHeaderColorProperty); }
            set { SetValue(BorderContentHeaderColorProperty, value); }
        }
        public static readonly DependencyProperty BorderContentHeaderColorProperty = DependencyProperty.Register(
            name: "BorderContentHeaderColor",
            propertyType: typeof(string),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata("#FF000000", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
        public double BorderContentMaxHeight
        {
            get { return (double)GetValue(BorderContentMaxHeightProperty); }
            set { SetValue(BorderContentMaxHeightProperty, value); }
        }
        public static readonly DependencyProperty BorderContentMaxHeightProperty = DependencyProperty.Register(
            name: "BorderContentMaxHeight",
            propertyType: typeof(double),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(double.MaxValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
        public double BorderContentMinHeight
        {
            get { return (double)GetValue(BorderContentMinHeightProperty); }
            set { SetValue(BorderContentMinHeightProperty, value); }
        }
        public static readonly DependencyProperty BorderContentMinHeightProperty = DependencyProperty.Register(
            name: "BorderContentMinHeight",
            propertyType: typeof(double),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
        /// <summary>
        /// 控制自身焦点
        /// </summary>
        public bool IsHitTestVisibleCallBack
        {
            get { return (bool)GetValue(IsHitTestVisibleCallBackProperty); }
            set { SetValue(IsHitTestVisibleCallBackProperty, value); }
        }
        public static readonly DependencyProperty IsHitTestVisibleCallBackProperty = DependencyProperty.Register(
            name: "IsHitTestVisibleCallBack",
            propertyType: typeof(bool),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
            {
                var control = (cPromptBox)d;
                var newValue = e.NewValue as bool?;
                if (newValue is true)
                {
                    control.Focus();//焦点甩过来
                }
            })
        );


        /// <summary>
        /// DataContext中转
        /// </summary>
        public object ItemModel
        {
            get { return (object)GetValue(ItemModelProperty); }
            set { SetValue(ItemModelProperty, value); }
        }
        public static readonly DependencyProperty ItemModelProperty = DependencyProperty.Register(
            name: "ItemModel",
            propertyType: typeof(object),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        /// <summary>
        /// DataContext中转
        /// </summary>
        public object ParentModel
        {
            get { return (object)GetValue(ParentModelProperty); }
            set { SetValue(ParentModelProperty, value); }
        }
        public static readonly DependencyProperty ParentModelProperty = DependencyProperty.Register(
            name: "ParentModel",
            propertyType: typeof(object),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }

    public partial class cPromptBox
    {
        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public new static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            name: "Text",
            propertyType: typeof(string),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) =>
            {
                if (e.NewValue is string text)
                {
                    var tb = (cPromptBox)s;
                    if (tb.IsManuallyEnabled && tb.TextBuffer != text)
                    {
                        tb.TextBuffer = text;
                    }
                }
            })
        );

        public string TextBuffer
        {
            get { return (string)GetValue(TextBufferProperty); }
            set { SetValue(TextBufferProperty, value); }
        }
        public static readonly DependencyProperty TextBufferProperty = DependencyProperty.Register(
            name: "TextBuffer",
            propertyType: typeof(string),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) =>
            {
                if (e.NewValue is string buffer)
                {
                    var tb = (cPromptBox)s;
                    if (tb.IsManuallyEnabled && tb.Text != buffer)
                    {
                        tb.Text = buffer;
                    }
                }
            })
        );

        public bool IsManuallyEnabled
        {
            get { return (bool)GetValue(IsManuallyEnabledProperty); }
            set { SetValue(IsManuallyEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsManuallyEnabledProperty = DependencyProperty.Register(
            name: "IsManuallyEnabled",
            propertyType: typeof(bool),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) =>
            {
                if (e.NewValue is bool flag)
                {
                    var tb = (cPromptBox)s;
                    if (flag && tb.Text != tb.TextBuffer) { tb.Text = tb.TextBuffer; }
                    if (flag is false)
                    {
                        tb.Text = string.Empty;
                    }
                }
            })
        );

        public bool UseOnOff
        {
            get { return (bool)GetValue(UseOnOffProperty); }
            set { SetValue(UseOnOffProperty, value); }
        }
        public static readonly DependencyProperty UseOnOffProperty = DependencyProperty.Register(
            name: "UseOnOff",
            propertyType: typeof(bool),
            ownerType: typeof(cPromptBox),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
