using TrarsUI.Shared.Interfaces.UIComponents;
using vNekoChatUI.A.MVVM.ViewModels;

namespace vNekoChatUI.A.Factories
{
    public class AContentProviderService : IContentProviderService
    {
        public IContentVM Create()
        {
            return new NekoChatVM();
        }
    }
}
