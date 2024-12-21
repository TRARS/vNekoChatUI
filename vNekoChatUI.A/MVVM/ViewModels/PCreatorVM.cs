using CommunityToolkit.Mvvm.ComponentModel;
using TrarsUI.Shared.Interfaces.UIComponents;
using vNekoChatUI.A.MVVM.Models;

namespace vNekoChatUI.A.MVVM.ViewModels
{
    partial class PCreatorVM : ObservableObject, IContentVM
    {
        public string Title { get; set; } = "PCreator";

        [ObservableProperty]
        private ContactModel bot;

    }
}
