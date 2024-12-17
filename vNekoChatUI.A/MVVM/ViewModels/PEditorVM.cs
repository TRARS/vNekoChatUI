using CommunityToolkit.Mvvm.ComponentModel;
using TrarsUI.Shared.Interfaces.UIComponents;
using vNekoChatUI.A.MVVM.Models;


namespace vNekoChatUI.A.MVVM.ViewModels
{
    partial class PEditorVM : ObservableObject, IContentVM
    {
        public string Title { get; set; } = "PEditor";

        [ObservableProperty]
        private ContactModel bot;
    }
}
