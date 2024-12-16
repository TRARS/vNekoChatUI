using TrarsUI.Shared.Interfaces.UIComponents;

namespace vNekoChatUI.TrarsWindow.MVVM.DesignTimeViewModels
{
    internal class uClientVM : IuClientVM
    {
        public object Content { get; set; }
        public string Token { get; set; }

        public void SetContent(object content) { }
    }
}
