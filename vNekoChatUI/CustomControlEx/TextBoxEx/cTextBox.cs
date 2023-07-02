using System.Windows;
using System.Windows.Controls;

namespace vNekoChatUI.CustomControlEx.TextBoxEx
{
    public partial class cTextBox : TextBox
    {
        static cTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cTextBox), new FrameworkPropertyMetadata(typeof(cTextBox)));
        }

        public cTextBox()
        {
            //this.DataContext = this;
        }
    }

    public partial class cTextBox
    {
        public string BorderBackground
        {
            get { return (string)GetValue(BorderBackgroundProperty); }
            set { SetValue(BorderBackgroundProperty, value); }
        }
        public static readonly DependencyProperty BorderBackgroundProperty = DependencyProperty.Register(
            name: "BorderBackground",
            propertyType: typeof(string),
            ownerType: typeof(cTextBox),
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
            ownerType: typeof(cTextBox),
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
            ownerType: typeof(cTextBox),
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
            ownerType: typeof(cTextBox),
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
            ownerType: typeof(cTextBox),
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
            ownerType: typeof(cTextBox),
            typeMetadata: new FrameworkPropertyMetadata(double.MaxValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
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
            ownerType: typeof(cTextBox),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
            {
                var control = (cTextBox)d;
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
            ownerType: typeof(cTextBox),
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
            ownerType: typeof(cTextBox),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
