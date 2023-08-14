using System.Windows;
using System.Windows.Controls;

namespace vNekoChatUI.CustomControlEx.GridEx
{
    public partial class cGrid : Grid
    {
        static cGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cGrid), new FrameworkPropertyMetadata(typeof(cGrid)));
        }
    }

    public partial class cGrid
    {
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
            ownerType: typeof(cGrid),
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
            ownerType: typeof(cGrid),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        /// <summary>
        /// cTextBox中转
        /// </summary>
        public object Inner_cTextBox
        {
            get { return (object)GetValue(Inner_cTextBoxProperty); }
            set { SetValue(Inner_cTextBoxProperty, value); }
        }
        public static readonly DependencyProperty Inner_cTextBoxProperty = DependencyProperty.Register(
            name: "Inner_cTextBox",
            propertyType: typeof(object),
            ownerType: typeof(cGrid),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
