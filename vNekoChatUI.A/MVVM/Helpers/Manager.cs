using CommunityToolkit.Mvvm.Messaging;
using TrarsUI.Shared.Messages;
using vNekoChatUI.A.MVVM.Models;
using vNekoChatUI.A.MVVM.ViewModels;

namespace vNekoChatUI.A.MVVM.Helpers
{
    internal static class Manager
    {
        public static void OpenPEditor(ContactModel selectedContact)
        {
            WeakReferenceMessenger.Default.Send(new OpenChildFormMessage(new()
            {
                Icon = "M0.986196 969.101766L89.250722 598.292135 647.108828 40.434029c25.31236-25.31236 59.500482-39.447833 95.167898-39.447833 35.667416 0 70.019904 14.135474 95.167897 39.447833l132.807704 132.807705c25.31236 25.31236 39.447833 59.500482 39.447834 95.167897 0 35.667416-14.135474 70.019904-39.447834 95.167897L412.394222 921.435634 41.420225 1009.700161c-11.341252 2.629856-23.339968-0.657464-31.558267-8.875763-8.218299-8.382665-11.669984-20.38138-8.875762-31.722632z m633.630818-54.405137h430.803211V1022.35634H634.617014v-107.659711z",
                ViewModel = new PEditorVM()
                {
                    Bot = selectedContact,
                },
                WindowInfo = new()
                {
                    MinHeight = 608,
                    MaxHeight = 608
                }
            }));
        }
    }
}
