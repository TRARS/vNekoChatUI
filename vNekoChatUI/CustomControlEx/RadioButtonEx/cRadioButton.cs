using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace vNekoChatUI.CustomControlEx.RadioButtonEx
{
    //构造函数
    public partial class cRadioButton : RadioButton
    {
        static cRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cRadioButton), new FrameworkPropertyMetadata(typeof(cRadioButton)));
        }
        public cRadioButton()
        {
            this.GroupName = "On(X7y^kjE1%LVR_";
        }
    }

    //依赖属性拿去绑定用
    public partial class cRadioButton
    {
        public string BorderMouseOverColor
        {
            get { return (string)GetValue(BorderMouseOverColorProperty); }
            set { SetValue(BorderMouseOverColorProperty, value); }
        }
        public static readonly DependencyProperty BorderMouseOverColorProperty = DependencyProperty.Register(
            name: "BorderMouseOverColor",
            propertyType: typeof(string),
            ownerType: typeof(cRadioButton),
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
            ownerType: typeof(cRadioButton),
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
            ownerType: typeof(cRadioButton),
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
            ownerType: typeof(cRadioButton),
            typeMetadata: new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
