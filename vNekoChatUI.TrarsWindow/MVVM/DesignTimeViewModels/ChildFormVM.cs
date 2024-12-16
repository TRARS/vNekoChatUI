using System.Collections.ObjectModel;
using TrarsUI.Shared.Interfaces.UIComponents;

namespace vNekoChatUI.TrarsWindow.MVVM.DesignTimeViewModels
{
    internal class ChildFormVM : IChildFormVM
    {
        public ObservableCollection<IToken> SubViewModelList { get; init; } = new()
        {
            new uTitleBarVM(),
            new uRainbowLineVM(),
            new uClientVM(),
        };
    }
}
