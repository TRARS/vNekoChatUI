using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace vNekoChatUI.Web.BlazorServer.Hubs
{
    public partial class ChatHub : Hub
    {
        static string? serverId;

        //服务端ID注册一下
        public async Task RegistersServerId(string _serverId)
        {
            if (string.IsNullOrWhiteSpace(_serverId) || (serverId == _serverId)) { return; }

            serverId = _serverId;
            Debug.WriteLine($"server[{serverId}] is online\n");
            await Task.CompletedTask;
        }

        //消息转发给服务端（供客户端执行服务端上指定的方法）
        public async Task ExecuteServerCommandWithName(byte[] method_content)
        {
            if (string.IsNullOrWhiteSpace(serverId) is false)
            {
                await Clients.Client(serverId).SendAsync($"ExecuteServerCommandWithName", method_content);
            }
        }

        //消息转发给客户端（供服务端执行客户端上指定的方法）
        public async Task ExecuteClientCommandWithName(byte[] method_content)
        {
            await Clients.Others.SendAsync("ExecuteClientCommandWithName", method_content);
        }
    }
}
