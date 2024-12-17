using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;

namespace vNekoChatUI.A.CustomControlEx.SuggestionMenuItemEx
{
    public partial class cSuggestionMenuItem : MenuItem
    {
        static cSuggestionMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cSuggestionMenuItem), new FrameworkPropertyMetadata(typeof(cSuggestionMenuItem)));
        }
    }

    public partial class cSuggestionMenuItem
    {
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            name: "Content",
            propertyType: typeof(object),
            ownerType: typeof(cSuggestionMenuItem),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public string BackgroundColor
        {
            get { return (string)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            name: "BackgroundColor",
            propertyType: typeof(string),
            ownerType: typeof(cSuggestionMenuItem),
            typeMetadata: new FrameworkPropertyMetadata("#FF000000", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public RelayCommand SelectSuggestionCommand
        {
            get { return (RelayCommand)GetValue(SelectSuggestionCommandProperty); }
            set { SetValue(SelectSuggestionCommandProperty, value); }
        }
        public static readonly DependencyProperty SelectSuggestionCommandProperty = DependencyProperty.Register(
            name: "SelectSuggestionCommand",
            propertyType: typeof(RelayCommand),
            ownerType: typeof(cSuggestionMenuItem),
            typeMetadata: new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );


        public bool LightOnOff
        {
            get { return (bool)GetValue(LightOnOffProperty); }
            set { SetValue(LightOnOffProperty, value); }
        }
        public static readonly DependencyProperty LightOnOffProperty = DependencyProperty.Register(
            name: "LightOnOff",
            propertyType: typeof(bool),
            ownerType: typeof(cSuggestionMenuItem),
            typeMetadata: new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );
    }
}
