using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using vNekoChatUI.Character.BingUtils;
using vNekoChatUI.Character.ChatGptUtils;
using vNekoChatUI.Character.GeminiUtils;
using vNekoChatUI.Character.SocketUtils;

namespace vNekoChatUI.Character
{
    //Sender资料
    public record SenderInfo([property: JsonPropertyName("UserName")] string UserName,
                             //[property: JsonPropertyName("UsernameColor")] string UsernameColor,
                             [property: JsonPropertyName("UserborderColor")] string UserborderColor,
                             [property: JsonPropertyName("ImageSource")] string ImageSource,
                             //[property: JsonPropertyName("Message")] string Message,
                             //[property: JsonPropertyName("TokenPrice")] string TokenPrice,
                             //[property: JsonPropertyName("Time")] string Time,
                             [property: JsonPropertyName("IsBot")] bool IsBot,
                             [property: JsonPropertyName("DisplayName")] string DisplayName);

    //ClientMessage 转换为 ChatMessage 再返回给上层
    public class ChatMessage
    {
        [JsonPropertyName("SenderName")]
        public string SenderName { get; set; } = string.Empty;

        [JsonPropertyName("SenderMessage")]
        public string SenderMessage { get; set; } = string.Empty;

        [JsonPropertyName("ReceiverMessage")]
        public string ReceiverMessage { get; set; } = string.Empty;

        [JsonPropertyName("TotalTokens")]
        public int TotalTokens { get; set; } = 0;

        public SenderInfo? SenderInfo { get; set; } = null;


        //序列化
        public string GetJsonString()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(this, options);
        }

        //反序列化
        public ChatMessage GetJsonObject(string jsonText)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            try
            {
                var jsonObject = JsonSerializer.Deserialize(jsonText, typeof(ChatMessage), options);

                if (jsonObject is not null)
                {
                    this.SenderName = ((ChatMessage)jsonObject).SenderName;
                    this.SenderMessage = ((ChatMessage)jsonObject).SenderMessage;
                    this.ReceiverMessage = ((ChatMessage)jsonObject).ReceiverMessage;
                    this.TotalTokens = ((ChatMessage)jsonObject).TotalTokens;
                }

                return this;
            }
            catch
            {
                this.SenderName = "error";
                this.SenderMessage = "error";
                this.ReceiverMessage = "JsonSerializer.Deserialize GeminiError";
                this.TotalTokens = -998;
                System.Windows.MessageBox.Show("CharacterWapper GeminiError");
                return this;
            }
        }
    }



    //Socket服务端
    public class ChatServerWapper
    {
        ChatServer _chatServer;//下层Socket服务端

        public ChatServerWapper(string _serverIP, int _serverPort, Action<int> callback, string name)
        {
            _chatServer = new ChatServer(name);
            _ = _chatServer.StartListening(_serverIP, _serverPort, callback);
        }

        /// <summary>
        /// 广播消息给所有在线客户端
        /// </summary>
        public void Broadcast(string message)
        {
            _chatServer.BroadcastMessageToAllClients(message);
        }

        /// <summary>
        /// 发送消息给指定客户端
        /// </summary>
        public void SendTo(string targetName, string message)
        {
            _chatServer.SendMessageToClient(targetName, message);
        }
    }



    //Socket客户端
    public class ChatClientWapper
    {
        private ChatClient _chatClient;//下层Socket客户端
        private bool _autoReply; //判断身份，Player不需要自动回复，仅BOT需要自动回复

        private Action<int> _stepUpCallBack;
        private Action<string> _autoReplyCallBack;//将ChatGPT（HTTP）客户端消息返回上层
        private Action<string> _receiveMessageCallBack;//将Socket客户端收到的消息返回上层（写入聊天记录）
        private Func<string> _getChatHistoryCallBack;//从上层获取聊天记录
        private Func<int> _getDebugMode;//从上层获取当前DebugMode状态

        private ChatGptUtils.HttpClientWapper _chatGptClient = new();//下层Http客户端（ChatGPT）
        private BingUtils.HttpClientWapper _bingGptClient = new();//下层Http客户端（NewBing）
        private GeminiUtils.HttpClientWapper _geminiClient = new();//下层Http客户端（NewBing）

        private Func<CancellationToken> _getCancellationToken;//取消任务

        public ChatClientWapper(Func<CancellationToken> getCancellationToken,
                                string name,
                                bool autoReply,
                                string serverIP,
                                int serverPort,
                                Action<int> stepUpCallBack,
                                Action<string> autoReplyCallBack,
                                Action<string> receiveMessageCallBack,
                                Func<string> getChatHistoryCallBack,
                                Func<int> getDebugMode)
        {
            _getCancellationToken = getCancellationToken;

            _chatClient = new ChatClient(name);
            _chatClient.StartClient(serverIP, serverPort, ClientCallBack);//这个回调函数是关键
            _autoReply = autoReply;

            _stepUpCallBack = stepUpCallBack;
            _autoReplyCallBack = autoReplyCallBack;
            _receiveMessageCallBack = receiveMessageCallBack;
            _getChatHistoryCallBack = getChatHistoryCallBack;
            _getDebugMode = getDebugMode;
        }

        /// <summary>
        /// 下层Socket客户端收到消息后会执行该回调函数
        /// </summary>
        private async void ClientCallBack(ClientMessage senderJson)//OnMessageReceived
        {
            //收件人收到其他人的Socket客户端发来的消息
            //var receivedMessage = senderJson.Message.TrimEnd("<EOF>".ToCharArray());//拆掉末端<EOF>

            //即将返回的对象的基本属性
            var chatMessage = new ChatMessage()
            {
                SenderName = senderJson.SenderName,
                SenderMessage = senderJson.Message,
                ReceiverMessage = string.Empty,
                TotalTokens = -999
            };

            //通知上层将收到的消息写入聊天记录
            _receiveMessageCallBack?.Invoke(chatMessage.GetJsonString());

            //判断是否DebugMode
            //int chatMode = _getDebugMode.Invoke();
            //LogProxy.Instance.Print($"this.DebugMode = {(int)(chatMode)}");
            switch (_getDebugMode.Invoke())
            {
                //debug
                case 0:
                    if (true)
                    {
                        _autoReplyCallBack?.Invoke(chatMessage.GetJsonString());//DebugMode直接返回
                    }
                    return;

                //chatgpt
                case 1:
                    //1. 若自己是BOT，则将收到的消息发送至 ChatGPT客户端，并等待回复
                    //2. 执行上层的回调函数，将 ChatGPT的回复 返回给上层
                    if (_autoReply && string.IsNullOrWhiteSpace(chatMessage.SenderMessage) is false)
                    {
                        await Task.Run(async () =>
                        {
                            try
                            {
                                //获取历史记录
                                var history = _getChatHistoryCallBack.Invoke();
                                //拿到ChatGPT回复
                                var jsonString = await _chatGptClient.Entry(history);

                                //解析json，TotalTokens: 本次消耗, Message: 本次回复
                                var jsonObject = JsonSerializer.Deserialize<Ai_Response>(jsonString);

                                chatMessage.ReceiverMessage = jsonObject.Message;
                                chatMessage.TotalTokens = jsonObject.TotalTokens;
                                _autoReplyCallBack?.Invoke(chatMessage.GetJsonString());
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"ClientCallBack GeminiError {ex.Message}");
                            }
                        });
                    }
                    return;

                //newbing
                case 2:
                    //1. 若自己是BOT，则将收到的消息发送至 Bing客户端，并等待回复
                    //2. 执行上层的回调函数，将 Bing的回复 返回给上层
                    if (_autoReply && string.IsNullOrWhiteSpace(chatMessage.SenderMessage) is false)
                    {
                        await Task.Run(async () =>
                        {
                            try
                            {
                                //获取历史记录
                                var history = _getChatHistoryCallBack.Invoke();
                                //拿到Bing回复
                                var jsonString = await _bingGptClient.Entry(history, _getCancellationToken, _stepUpCallBack);

                                //解析jsonn
                                var jsonObject = JsonSerializer.Deserialize<Bing_Response>(jsonString);

                                chatMessage.ReceiverMessage = jsonObject.Message;
                                chatMessage.TotalTokens = jsonObject.TotalTokens;
                                //返回
                                _autoReplyCallBack?.Invoke(chatMessage.GetJsonString());
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"ClientCallBack GeminiError {ex.Message}");
                            }
                        });

                    }
                    return;

                //gemini
                case 3:
                    if (_autoReply && string.IsNullOrWhiteSpace(chatMessage.SenderMessage) is false)
                    {
                        await Task.Run(async () =>
                        {
                            try
                            {
                                //获取历史记录
                                var history = _getChatHistoryCallBack.Invoke();
                                //拿到Gemini回复
                                var jsonString = await _geminiClient.Entry(history, _getCancellationToken);

                                //解析json，TotalTokens: 本次消耗, Message: 本次回复
                                var jsonObject = JsonSerializer.Deserialize<Gemini_Response>(jsonString);

                                chatMessage.ReceiverMessage = jsonObject.Message;
                                chatMessage.TotalTokens = jsonObject.TotalTokens;
                                _autoReplyCallBack?.Invoke(chatMessage.GetJsonString());
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"ClientCallBack GeminiError {ex.Message}");
                            }
                        });
                    }
                    return;
            }
        }

        /// <summary>
        /// 发送消息给指定客户端
        /// </summary>
        public async Task SendAsync(string targetName, string message)
        {
            await _chatClient.SendMessageToClient(targetName, message);
        }
    }
}
