using CommunityToolkit.Mvvm.ComponentModel;
using TrarsUI.Shared.Interfaces.UIComponents;
using vNekoChatUI.UserControlEx.ClientEx;

namespace vNekoChatUI.Test.PromptEditor.MVVM.ViewModels
{
    partial class PEditorVM : ObservableObject, IContentVM
    {
        public string Title { get; set; } = "PEditor";

        [ObservableProperty]
        private ContactModel bot;
    }
}
