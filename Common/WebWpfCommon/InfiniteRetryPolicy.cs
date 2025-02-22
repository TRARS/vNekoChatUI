using Microsoft.AspNetCore.SignalR.Client;

namespace Common.WebWpfCommon
{
    /// <summary>
    /// 重连规则
    /// </summary>
    public class InfiniteRetryPolicy : IRetryPolicy
    {
        private const int ReconnectionWaitSeconds = 10;
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(ReconnectionWaitSeconds);
        }
    }
}
