using TrarsUI.Shared.Interfaces.UIComponents;

namespace vNekoChatUI.TrarsWindow.MVVM.DesignTimeViewModels
{
    internal class uTitleBarVM : IuTitleBarVM
    {
        public string Title { get; set; } = "Designtime uTitleBar";
        public string Token { get; set; } = string.Empty;
    }
}
