namespace vNekoChatUI.Web.BlazorServer.WebUI.Service
{
    public class MessageService
    {
        public event Func<Task>? OnChatRoomUpdated;
        public async Task NotifyChatRoomUpdated()
        {
            if (OnChatRoomUpdated != null)
            {
                await OnChatRoomUpdated.Invoke();
            }
        }

        public event Func<Task>? OnNavMenuUpdated;
        public async Task NotifyNavMenuUpdated()
        {
            if (OnNavMenuUpdated != null)
            {
                await OnNavMenuUpdated.Invoke();
            }
        }

        public event Func<Task>? HideNavigationBar;
        public async Task NotifyHideNavigationBar()
        {
            if (HideNavigationBar != null)
            {
                await HideNavigationBar.Invoke();
            }
        }

        public event Func<Task>? HideInputBox;
        public async Task NotifyHideInputBox()
        {
            if (HideInputBox != null)
            {
                await HideInputBox.Invoke();
            }
        }
    }
}
