using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using TrarsUI.Shared.Helper.Extensions;
using TrarsUI.Shared.Interfaces.UIComponents;

namespace vNekoChatUI.A.MVVM.ViewModels
{
    partial class AppIconVM : ObservableObject, IContentVM
    {
        public string Title { get; set; } = "AppIcon";

        [ObservableProperty]
        private Geometry iconData;

        public AppIconVM()
        {
            var icon = "M853.333333 85.333333 170.666667 85.333333C123.733333 85.333333 85.333333 123.733333 85.333333 170.666667l0 768 170.666667-170.666667 597.333333 0c46.933333 0 85.333333-38.4 85.333333-85.333333L938.666667 170.666667C938.666667 123.733333 900.266667 85.333333 853.333333 85.333333zM256 384l512 0 0 85.333333L256 469.333333 256 384zM597.333333 597.333333 256 597.333333l0-85.333333 341.333333 0L597.333333 597.333333zM768 341.333333 256 341.333333 256 256l512 0L768 341.333333z";
            this.IconData = Geometry.Parse(icon.ToString(CultureInfo.InvariantCulture));
        }

        [RelayCommand]
        private void OnSaveIcon(object para)
        {
            if (para is UIElement control)
            {
                control.SaveToPng(allowTransparency: true);
                Debug.WriteLine($"OnSaveIcon: {control}");
            }
        }
    }
}
