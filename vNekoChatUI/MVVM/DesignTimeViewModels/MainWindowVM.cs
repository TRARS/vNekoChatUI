using System.Collections.ObjectModel;
using TrarsUI.Shared.Interfaces.UIComponents;

namespace vNekoChatUI.MVVM.DesignTimeViewModels
{
    internal class MainWindowVM : IMainWindowVM
    {
        public ObservableCollection<IToken> SubViewModelList { get; init; } = new()
        {
            App.GetRequiredService<IuTitleBarVM>(),
            App.GetRequiredService<IuRainbowLineVM>(),
            App.GetRequiredService<IuClientVM>(),
        };
    }
}
