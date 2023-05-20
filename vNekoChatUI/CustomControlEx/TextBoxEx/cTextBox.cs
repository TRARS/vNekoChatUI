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
    }
}
