using System.Windows;
using System.Windows.Controls;
using vNekoChatUI.Base.Helper.Extensions;

namespace vNekoChatUI.CustomControlEx.ListViewChatHistoryEx
{
    public partial class cListViewChatHistory : ListView
    {
        static cListViewChatHistory()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cListViewChatHistory), new FrameworkPropertyMetadata(typeof(cListViewChatHistory)));
        }
    }

    public partial class cListViewChatHistory
    {
        public void CaptureScreenshot()
        {
            var csv = this.FindVisualChild<cScrollViewer>();
            csv.CaptureScreenshot();
        }
    }
}
