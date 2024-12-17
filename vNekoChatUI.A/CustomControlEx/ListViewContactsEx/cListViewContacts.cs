using System.Windows;
using System.Windows.Controls;

namespace vNekoChatUI.A.CustomControlEx.ListViewContactsEx
{
    public class cListViewContacts : ListView
    {
        static cListViewContacts()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cListViewContacts), new FrameworkPropertyMetadata(typeof(cListViewContacts)));
        }
    }
}
