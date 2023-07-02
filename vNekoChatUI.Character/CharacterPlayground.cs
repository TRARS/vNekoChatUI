using System;
using System.Collections.Generic;
using System.Threading;

namespace vNekoChatUI.Character
{
    public partial class CharacterPlayground
    {
        private string serverIP = "127.0.0.1";
        private int serverPort = int.MinValue;

        private Dictionary<string, Func<SenderInfo>> loginUserList = new();

        /// <summary>
        /// 新建服务端
        /// </summary>
        public ChatServerWapper CreateChatServer(string _serverIP, int _serverPort, string _name)
        {
            serverIP = _serverIP;
            return new ChatServerWapper(_serverIP, _serverPort, (p) => { serverPort = p; }, _name);
        }

        /// <summary>
        /// 新建客户端（在创建客户端之前必须先创建一个服务端，否则无法获知服务端实际使用的端口）
        /// </summary>
        public ChatClientWapper CreateChatClient(Func<CancellationToken> _getCancellationToken,
                                                 string _name,
                                                 bool _autoReply,
                                                 Action<int> _stepUpCallBack,
                                                 Action<string> _autoReplyCallBack,
                                                 Action<string> _receiveMessageCallBack,
                                                 Func<string> _getChatHistoryCallBack,
                                                 Func<int> _getDebugMode,
                                                 Func<SenderInfo> _getUserProfileCallBack)
        {
            //用户登录至游乐场
            loginUserList.TryAdd(_name, _getUserProfileCallBack);

            //返回该用户的客户端（没有防呆
            return new ChatClientWapper(_getCancellationToken,
                                        _name, _autoReply, serverIP, serverPort,
                                        _stepUpCallBack,
                                        _autoReplyCallBack,
                                        _receiveMessageCallBack,
                                        _getChatHistoryCallBack,
                                        _getDebugMode);
        }
    }

    public partial class CharacterPlayground
    {
        public SenderInfo FindOnlineUserByName(string userName)
        {
            if (loginUserList.ContainsKey(userName))
            {
                var obj = loginUserList[userName].Invoke();
                return obj;
            }

            //可能是匿名用户
            return new SenderInfo("Anonymous", "#FF0000", string.Empty, true, "Anonymous");
        }
    }
}
