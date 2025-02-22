using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Common.WebWpfCommon
{
    public partial class ChatHub : Hub
    {
        // 用于存储当前的 ChatCenterId 和其对应的连接ID
        static string? _currentCenterId;

        // HubContext 用于控制其他连接
        private readonly IHubContext<ChatHub> _hubContext;

        // 定番方法
        public static string ReceiveMessage = nameof(ReceiveMessage);
        public static string ForceDisconnect = nameof(ForceDisconnect);

        /// <summary>
        /// 构造
        /// </summary>
        public ChatHub(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// 注册具体的 HubConnection.ConnectionId 为ChatCenter
        /// </summary>
        public async Task RegistersServerId(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId) || (_currentCenterId == clientId))
            {
                return; // 如果 clientId 未变动则直接返回
            }

            // 如果有旧的 ChatCenter，断开它
            if (_currentCenterId != null)
            {
                // 主动断开旧连接
                await _hubContext.Clients.Client(_currentCenterId).SendAsync(ChatHub.ForceDisconnect, Context.ConnectionId);
            }

            _currentCenterId = clientId;
            Debug.WriteLine($"server[{_currentCenterId}] is online\n");
        }

        /// <summary>
        /// 私聊
        /// </summary>
        public async Task SendPrivateMessage(byte[] method_content, string? clientId = null)
        {
            var targetId = clientId ?? _currentCenterId;

            if (string.IsNullOrWhiteSpace(targetId) is false)
            {
                try
                {
                    await Clients.Client(targetId).SendAsync(ChatHub.ReceiveMessage, Context.ConnectionId, method_content);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error sending message to client {targetId}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 群聊
        /// </summary>
        public async Task SendPublicMessage(byte[] method_content)
        {
            try
            {
                //Debug.WriteLine($"（Hub转发消息前）应当排除:\n——center({_currentCenterId})\n——sender({Context.ConnectionId})");
                await Clients.AllExcept([_currentCenterId, Context.ConnectionId]).SendAsync(ChatHub.ReceiveMessage, Context.ConnectionId, method_content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending message to other clients: {ex.Message}");
            }
        }
    }
}
