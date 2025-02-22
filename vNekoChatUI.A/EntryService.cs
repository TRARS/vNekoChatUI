using Microsoft.Extensions.DependencyInjection;
using System;
using TrarsUI.Shared.Helpers.Extensions;
using TrarsUI.Shared.Interfaces.UIComponents;
using vNekoChatUI.A.MVVM.Helpers;
using vNekoChatUI.A.MVVM.ViewModels;

namespace vNekoChatUI.A
{
    public class EntryService : IContentProviderService
    {
        private readonly IServiceProvider _serviceProvider;

        public EntryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IContentVM Create()
        {
            return _serviceProvider.GetRequiredService<NekoChatVM>();
        }

        public static void Register(IServiceCollection services)
        {
            services.AddFormFactory<NekoChatVM>();
            services.AddFormFactory<PEditorVM>();
            services.AddSingleton<Manager>();
        }
    }
}
