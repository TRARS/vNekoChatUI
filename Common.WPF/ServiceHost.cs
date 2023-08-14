
using Common.WPF.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Common.WPF
{
    //单例
    public partial class ServiceHost
    {
        private static readonly object objlock = new object();
        private static ServiceHost? _instance;
        public static ServiceHost Instance
        {
            get
            {
                lock (objlock)
                {
                    if (_instance is null)
                    {
                        _instance = new ServiceHost();
                    }
                }
                return _instance;
            }
        }
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
