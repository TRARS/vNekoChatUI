#if DEBUG
#else
#endif

using System;
using System.Diagnostics;
using System.Windows;
using TrarsUI.Shared.Collections;

namespace Common.WPF
{
    //单例
    public partial class LogProxy
    {
        private static readonly Lazy<LogProxy> lazyObject = new(() => new LogProxy());
        public static LogProxy Instance => lazyObject.Value;
    }

    //公开
    public partial class LogProxy
    {
        private readonly LimitedSizeObservableCollection<string> _logListReversed = new(15, 1);
        public LimitedSizeObservableCollection<string> LogContentReversed => _logListReversed;

        public void Print(string message)
        {
            // 在主线程上更新UI
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                _logListReversed.AddToBeginning($"{message}");
            });

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Debug.WriteLine(message);
            }
        }
    }
}
