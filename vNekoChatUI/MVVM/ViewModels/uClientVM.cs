using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;

namespace vNekoChatUI.MVVM.ViewModels
{
    partial class uClientVM : ObservableObject, IuClientVM
    {
        private IMessageBoxService _messageBox;

        [ObservableProperty]
        private object content;

        [ObservableProperty]
        private DataTemplateSelector contentTemplateSelector;

        [ObservableProperty]
        private string token;

        public uClientVM(IMessageBoxService messageBox)
        {
            this._messageBox = messageBox;
        }
    }

    partial class uClientVM
    {
        partial void OnTokenChanged(string value)
        {
            //var token = value;

            //WeakReferenceMessenger.Default.Register<SomeMessage, string>(this, token, (r, m) =>
            //{

            //});
        }

        public void SetContent(object content)
        {
            this.Content = content;
        }
    }
}
