using System.Windows;
using System.Windows.Controls.Primitives;

namespace vNekoChatUI.A.CustomControlEx.ToggleButtonEx
{
    public partial class cToggleButton : ToggleButton
    {
        static cToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cToggleButton), new FrameworkPropertyMetadata(typeof(cToggleButton)));
        }
    }

    public partial class cToggleButton
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            name: "Text",
            propertyType: typeof(string),
            ownerType: typeof(cToggleButton),
            typeMetadata: new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            name: "Radius",
            propertyType: typeof(double),
            ownerType: typeof(cToggleButton),
            typeMetadata: new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            name: "Diameter",
            propertyType: typeof(double),
            ownerType: typeof(cToggleButton),
            typeMetadata: new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public bool Enable
        {
            get { return (bool)GetValue(EnableProperty); }
            set { SetValue(EnableProperty, value); }
        }
        public static readonly DependencyProperty EnableProperty = DependencyProperty.Register(
            name: "Enable",
            propertyType: typeof(bool),
            ownerType: typeof(cToggleButton),
            typeMetadata: new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
