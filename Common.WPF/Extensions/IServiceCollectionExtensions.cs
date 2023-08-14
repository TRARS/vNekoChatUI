using Common.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace Common.WPF.Extensions
{
    //  ServiceCollection拓展方法
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection TryAddAllSingletonService(this IServiceCollection services)
        {
            // 注册服务
            services.TryAddSingleton<IBingVisualSearchService, BingVisualSearchService>();
            services.TryAddSingleton<IBmpService, BmpService>();
            services.TryAddSingleton<IDefWebService, DefWebService>();
            services.TryAddSingleton<IFlagService, FlagService>();
            services.TryAddSingleton<ISharpTokenService, SharpTokenService>();
            services.TryAddSingleton<ISignalRClientService, SignalRClientService>();
            services.TryAddSingleton<IStreamService, StreamService>();

            //
            return services;
        }
    }
}
