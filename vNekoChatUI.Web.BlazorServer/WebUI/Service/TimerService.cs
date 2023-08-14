using Microsoft.AspNetCore.Components;

namespace vNekoChatUI.Web.BlazorServer.WebUI.Service
{
    public partial class TimerService : ComponentBase
    {
        /// <summary>
        /// 内部计时器类
        /// </summary>
        public sealed class InnerTimer
        {
            public int Count = 0;
            public event Func<Task> OnInput;

            bool flag = false;
            int elapsed = 0;

            public bool CoolDown(int _threshold)
            {
                elapsed = 0;//_threshold毫秒内连续访问就直接计数器归零

                if (flag is false)
                {
                    flag = true;

                    Task.Run(async () =>
                    {
                        int countdownDuration = _threshold; // 5 seconds
                        int interval = 100; // 100 milliseconds

                        while (elapsed < countdownDuration)
                        {
                            // 检测是否已经过了_threshold毫秒
                            if (elapsed >= countdownDuration)
                            {
                                break;
                            }

                            // 延迟100毫秒
                            await Task.Delay(interval);

                            // 更新已经过去的时间
                            elapsed += interval;
                        }

                        // 倒计时结束后执行的操作
                        flag = false; elapsed = 0;

                        if (OnInput is not null)
                        {
                            await OnInput.Invoke();
                        }

                        return true;
                    });
                }

                return false;
            }
        }
    }

    public partial class TimerService
    {
        /// <summary>
        /// 内部字典
        /// </summary>
        private readonly Dictionary<string, InnerTimer> internalList = new();

        /// <summary>
        /// 通过key访问内部计时器
        /// </summary>
        public InnerTimer this[string key]
        {
            get
            {
                if (internalList.ContainsKey(key) is false)
                {
                    internalList.Add(key, new());
                }
                return internalList[key];
            }
        }
    }
}
