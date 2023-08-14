using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using vNekoChatUI.Character.SocketUtils.Service;

namespace vNekoChatUI.Character.SocketUtils.Extensions
{
    //  ServiceCollection拓展方法
    public static class ServiceCollectionExtensions
    {
        //// 每次使用完（请求结束时）即释放
        //services.AddTransient<Service1>();
        //// 超出范围（请求结束时）则释放
        //services.AddScoped<Service2>();
        //// 程序停止时释放
        //services.AddSingleton<Service3>();
        //// 程序停止时释放
        //services.AddSingleton(sp => new Service4());


        public static IServiceCollection TryAddJsonService(this IServiceCollection services)
        {
            // 注册Json服务
            services.TryAddSingleton<IJsonService, JsonService>();
            return services;
        }

        //public static IServiceCollection TryAddQueueService(this IServiceCollection services)
        //{
        //    // 注册Queue服务
        //    services.TryAddSingleton<IQueueService, QueueService>();
        //    return services;
        //}
    }
}
