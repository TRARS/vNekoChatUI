using TrarsUI.Shared.Interfaces.UIComponents;
using vNekoChatUI.A.MVVM.ViewModels;

namespace vNekoChatUI.A.Factories
{
    public class AContentProviderService : IContentProviderService
    {
        public string Title { get; set; }

        public IContentVM Create()
        {
            IContentVM result = new NekoChatVM();

            this.Title = result.Title;

            return result;
        }
    }
}
