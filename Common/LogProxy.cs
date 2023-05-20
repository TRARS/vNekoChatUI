#if DEBUG
global using Console = System.Diagnostics.Debug;
#else
global using Console = System.Diagnostics.Trace;
#endif

using System.Collections.ObjectModel;
using System.Threading;

namespace Common
{
    public sealed class LimitedSizeObservableCollection<T> : ObservableCollection<T>
    {
        int Capacity { get; } = 15;
        public new void Add(T item)
        {
            if (Count >= Capacity) { this.RemoveAt(0); }
            base.Add(item);
        }
    }

    //单例
    public partial class LogProxy
    {
        private static readonly object objlock = new object();
        private static LogProxy? _instance;
        public static LogProxy Instance
        {
            get
            {
                lock (objlock)
                {
                    if (_instance is null)
                    {
                        _instance = new LogProxy();
                    }
                }
                return _instance;
            }
        }
    }

    //公开
    public partial class LogProxy
    {
        private SynchronizationContext? uiContext;
        private readonly LimitedSizeObservableCollection<string> _logList = new();

        public void Print(string message)
        {
            //Sent同步 Post异步，此处同步会死锁
            uiContext?.Post(state =>
            {
                _logList.Add($"{message}");
            }, null);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine(message);
            }
        }

        public LimitedSizeObservableCollection<string> GetLogContent()
        {
            return _logList;
        }

        public void SetUIContext(SynchronizationContext? uiContext)
        {
            this.uiContext = uiContext;
        }
    }
}
