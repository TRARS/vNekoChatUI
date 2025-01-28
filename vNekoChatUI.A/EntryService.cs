using TrarsUI.Shared.Interfaces.UIComponents;
using vNekoChatUI.A.MVVM.ViewModels;

namespace vNekoChatUI.A
{
    public class EntryService : IContentProviderService
    {
        public IContentVM Create()
        {
            return new NekoChatVM();
        }
    }
}
