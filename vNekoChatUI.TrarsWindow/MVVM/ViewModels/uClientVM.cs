using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;

namespace vNekoChatUI.TrarsWindow.MVVM.ViewModels
{
    public partial class uClientVM : ObservableObject, IuClientVM
    {
        private IMessageBoxService _messageBox;

        [ObservableProperty]
        public object content;

        [ObservableProperty]
        public DataTemplateSelector contentTemplateSelector;

        [ObservableProperty]
        public string token;

        public uClientVM(IMessageBoxService messageBox)
        {
            this._messageBox = messageBox;
        }
    }

    public partial class uClientVM
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
