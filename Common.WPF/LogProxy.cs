#if DEBUG
global using Console = System.Diagnostics.Debug;
#else
global using Console = System.Diagnostics.Trace;
#endif

using System.Collections.ObjectModel;
using System.Windows;

namespace Common.WPF
{
    public sealed class LimitedSizeObservableCollection<T> : ObservableCollection<T>
    {
        int Capacity { get; } = 15;
        public new void Add(T item)
        {
            if (Count >= Capacity) { this.RemoveAt(0); }
            base.Add(item);
        }

        public void AddToBeginning(T item)
        {
            if (Count >= Capacity) { this.RemoveAt(Capacity - 1); }
            base.Insert(0, item);
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
        private readonly LimitedSizeObservableCollection<string> _logListReversed = new();
        public LimitedSizeObservableCollection<string> LogContentReversed => _logListReversed;

        public void Print(string message)
        {
            // 在主线程上更新UI
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //_logList.Add($"{message}");
                _logListReversed.AddToBeginning($"{message}");
                //System.Console.WriteLine(message);
            });

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine(message);
            }
        }
    }
}
