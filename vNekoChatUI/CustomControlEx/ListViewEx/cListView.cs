using System.Windows;
using System.Windows.Controls;

namespace vNekoChatUI.CustomControlEx.ListViewEx
{
    public partial class cListView : ListView
    {
        static cListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cListView), new FrameworkPropertyMetadata(typeof(cListView)));
        }
    }
}
