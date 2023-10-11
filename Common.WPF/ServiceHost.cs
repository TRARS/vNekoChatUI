
using Common.WPF.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Common.WPF
{
    //单例
    public partial class ServiceHost
    {
        private static readonly Lazy<ServiceHost> lazyObject = new(() => new ServiceHost());
        public static ServiceHost Instance => lazyObject.Value;
    }

    public partial class ServiceHost
    {
        ServiceProvider _sp;

        public ServiceHost()
        {
            _sp = new ServiceCollection().TryAddAllSingletonService().BuildServiceProvider();
        }

        public T GetService<T>() where T : notnull
        {
            return _sp.GetRequiredService<T>();
        }
    }
}
