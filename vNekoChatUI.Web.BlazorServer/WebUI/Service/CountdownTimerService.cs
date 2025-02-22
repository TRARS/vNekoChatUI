using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace vNekoChatUI.Web.BlazorServer.WebUI.Service
{
    public class CountdownTimerService
    {
        private Timer _timer;
        private int _remainingSeconds;
        private readonly object _lock = new();

        public Action<int> OnTick;   // 每秒触发，通知剩余时间
        public Action<int> OnCompleted;   // 倒计时结束触发

        public CountdownTimerService(int seconds = 60)
        {
            _remainingSeconds = seconds;
            _timer = new Timer(1000);  // 1秒触发一次
            _timer.Elapsed += TimerElapsed;
        }

        public void Start()
        {
            lock (_lock)
            {
                if (!_timer.Enabled)
                {
                    _timer.Start();
                    Debug.WriteLine("倒计时开始...");
                }
            }
        }

        public void Reset(int seconds = 60)
        {
            lock (_lock)
            {
                _remainingSeconds = seconds;
                //Debug.WriteLine($"倒计时重置为 {seconds} 秒");
            }
        }

        public void ReStart(int seconds = 60, Action<int>? callback = null)
        {
            if (_timer.Enabled is false)
            {
                this.Start();

                OnTick = callback;
                OnCompleted = callback;
            }

            this.Reset(seconds);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_remainingSeconds > 0)
                {
                    _remainingSeconds--;
                    OnTick?.Invoke(_remainingSeconds);
                    //Debug.WriteLine($"剩余时间: {_remainingSeconds} 秒");

                    if (_remainingSeconds == 0)
                    {
                        _timer.Stop();
                        OnCompleted?.Invoke(_remainingSeconds);
                        //Debug.WriteLine("倒计时结束");
                    }
                }
            }
        }

        public void Stop()
        {
            _timer.Stop();
            Debug.WriteLine("倒计时停止");
        }
    }
}
