using Common.WebWpfCommon;

namespace vNekoChatUI.Web.BlazorServer.WebUI.MainViewModel
{
    public interface IChatRoomModel
    {
        //接收用
        string AiName { get; set; }
        string AiProfile { get; set; }
        string AiInnerMonologue { get; set; }
        List<Ai_Content> ChatHistory { get; set; }

        //发送用
        bool NeedRefreshPage { get; set; }
        bool HistoryChanged { get; set; }
        bool BingBypassDetection { get; set; }

        //
        bool IsDarkMode { get; set; }
        string connectionId { get; set; }
        //
        Action<string>? ReSend { get; set; }
        Action<bool, bool>? Screenshot_Windows { get; set; }
        Action<bool, bool>? Screenshot_SmartPhone { get; set; }
        Action<string>? RefreshProfile { get; set; }
        Action<Action<bool>?>? HideNavMenu { get; set; }
        Action<Action<bool>?>? HideInputBox { get; set; }
        Action? ColorChange { get; set; }
    }
    public partial class ChatRoomModel : IChatRoomModel
    {
        //
        public string AiName { get; set; } = "";
        public string AiProfile { get; set; } = "";
        public string AiInnerMonologue { get; set; } = "";
        public List<Ai_Content> ChatHistory { get; set; } = new();

        //
        public bool NeedRefreshPage { get; set; } = false;
        public bool HistoryChanged { get; set; } = false;
        public bool BingBypassDetection { get; set; } = false;

        //
        public bool IsDarkMode { get; set; } = true;
        public string connectionId { get; set; } = "";
        //
        public Action<string>? ReSend { get; set; } = null;
        public Action<bool, bool>? Screenshot_Windows { get; set; }
        public Action<bool, bool>? Screenshot_SmartPhone { get; set; }
        public Action<string>? RefreshProfile { get; set; } = null;
        public Action<Action<bool>?>? HideNavMenu { get; set; } = null;
        public Action<Action<bool>?>? HideInputBox { get; set; } = null;
        public Action? ColorChange { get; set; } = null;
    }
}
