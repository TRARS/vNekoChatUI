using System.Windows;
using System.Windows.Controls;

namespace vNekoChatUI.A.CustomControlEx.InputBoxEx
{
    public partial class cInputBox : TextBox
    {

    }
    public partial class cInputBox
    {
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            name: "Placeholder",
            propertyType: typeof(string),
            ownerType: typeof(cInputBox),
            typeMetadata: new FrameworkPropertyMetadata("empty", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
