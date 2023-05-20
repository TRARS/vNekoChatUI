using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using vNekoChatUI.Base.Helper;
using vNekoChatUI.Base.Helper.Generic;
using vNekoChatUI.Character;
using ChatMessage = vNekoChatUI.Character.ChatMessage;

namespace vNekoChatUI.UserControlEx.ClientEx
{
    internal class MessageModel
    {
        public string Username { get; set; }
        public string UsernameColor { get; set; }
        public string UserborderColor { get; set; }
        public string ImageSource { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        //public bool IsNativeOrigin { get; set; }
        //public bool? FirseMessage { get; set; }
        public int TokenPrice { get; set; } = 0;

        public bool IsBot { get; set; } = true;//影响聊天气泡位于左或右
    }

    internal partial class ContactModel : NotificationObject
    {
        //重要、不可修改
        private bool IsBot { get; init; }
        private ChatClientWapper ChatClient { get; init; }

        //不可修改
        public string Username { get; init; }
        public string Usertone { get; init; }
        //public string UserborderColor { get; init; }
        public string ImageSource { get; init; }
        public ObservableCollection<MessageModel> Messages { get; init; }
        public Func<string> Say { get; init; }



        //可修改
        public string Signature { get; set; }
        public bool IsHistoryChanged { get; set; }

        private string _profile;//玛德还是得上通知属性
        public string Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                NotifyPropertyChanged();
            }
        }
        private string _userborderColor;//
        public string UserborderColor
        {
            get { return _userborderColor; }
            set
            {
                _userborderColor = value;
                NotifyPropertyChanged();
            }
        }




        public CharacterPlayground ChatPlayground { get; init; }

        //构造函数
        public ContactModel(CharacterPlayground _chatPlayground, string _clientName, bool _isBot, Func<int> _isDebugMode)
        {
            this.IsBot = _isBot;
            this.ChatClient = _chatPlayground.CreateChatClient(_clientName,
                                                               _isBot,
                                                               AutoReplyCallBack,
                                                               ReceiveMessageCallBack,
                                                               GetChatHistoryCallBack,
                                                               _isDebugMode,
                                                               GetUserProfileCallBack);
            this.ChatPlayground = _chatPlayground;//
            this.IsHistoryChanged = false;
        }
    }
    internal partial class ContactModel
    {
        /// <summary>
        /// 将消息发送给 targetContact
        /// </summary>
        public async Task SendTo(ContactModel targetContact, string sender_said)
        {
            //发件人
            //var sender = this;

            //收件人
            var receiver = targetContact;

            //消息路径: -> 发件人Socket客户端 -> Socket服务端 -> 收件人Socket客户端
            await this.ChatClient.SendAsync(receiver.Username, sender_said);
        }

        /// <summary>
        /// 供viewmodel载入聊天记录
        /// </summary>
        public void LoadChatHistory(BingChatHistoryModel item)
        {
            if (this.IsBot)
            {
                // 汇总，写入自身聊天记录
                var messageModel = new MessageModel()
                {
                    Username = item.Username,
                    UsernameColor = "White",
                    UserborderColor = item.UserborderColor,
                    ImageSource = item.ImageSource,
                    Message = item.Message,
                    TokenPrice = -1,//发件方消耗的token不用管
                    Time = item.Time,
                    IsBot = item.IsBot,
                };

                //用Invoke同步
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    this.Messages.Add(messageModel);
                });
            }
        }

        //接收下层的回复
        private void AutoReplyCallBack(string response)
        {
            if (this.IsBot)
            {
                var chatMessage = new ChatMessage().GetJsonObject(response);

                //BOT的自动回复，写入聊天记
                {
                    var token = chatMessage.TotalTokens;
                    var message = chatMessage.ReceiverMessage;

                    //于DebugMode时，下层不会返回ChatGPT的回复，故此处使用随机句子作自动回复
                    if (string.IsNullOrWhiteSpace(message)) { message = $"DebugMode: {this.Say.Invoke()}"; }

                    var messageModel = new MessageModel()
                    {
                        Username = this.Username,
                        UsernameColor = "White",
                        UserborderColor = this.UserborderColor,
                        ImageSource = this.ImageSource,
                        Message = message,
                        TokenPrice = token,//本次答复消耗token
                        Time = DateTime.Now,
                        IsBot = this.IsBot,
                    };

                    //ObservableCollection<MessageModel> Messages 对象已绑定到UI，
                    //故在其他线程调用此处的回调函数时，只能委托UI线程去操作
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.Messages.Add(messageModel);
                    });
                }
            }
        }

        //供下层查询完整聊天记录
        private string GetChatHistoryCallBack()
        {
            if (this.IsBot is false) { return "I am Player"; }

            var message_history = this.Messages.Select(m => new
            {
                roles = (m.Username == this.Username ? "Assistant" : "User"),
                content = m.Message
            });
            var data_content = new
            {
                ai_name = this.Username,
                ai_profile = this.Profile.Trim(),//string.Format(this.Profile.Trim(), this.Username, this.Usertone),
                ai_content = message_history,
                history_changed = this.IsHistoryChanged
            };

            var json = JsonSerializer.Serialize(data_content, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            this.IsHistoryChanged = false;//使用完毕，复位

            return json;
        }

        //供下层查询用户资料
        private SenderInfo GetUserProfileCallBack()
        {
            return new SenderInfo(this.Username,
                                  this.UserborderColor,
                                  this.ImageSource,
                                  this.IsBot);
        }

        //供下层添加消息至聊天记录
        private void ReceiveMessageCallBack(string response)
        {
            if (this.IsBot)
            {
                ChatMessage chatMessage = new ChatMessage().GetJsonObject(response);

                //LogProxy.Instance.Print($"response = {response}");

                //BOT收到的消息，写入聊天记录
                {
                    // 通过 SenderName 查找它的资料
                    var sender = this.ChatPlayground.FindOnlineUserByName(chatMessage.SenderName);
                    var sender_said = chatMessage.SenderMessage;

                    // 汇总，写入自身聊天记录
                    var messageModel = new MessageModel()
                    {
                        Username = sender.UserName,
                        UsernameColor = "White",
                        UserborderColor = sender.UserborderColor,
                        ImageSource = sender.ImageSource,
                        Message = sender_said,
                        TokenPrice = -1,//发件方消耗的token不用管
                        Time = DateTime.Now,
                        IsBot = sender.IsBot,
                    };

                    //用Invoke同步
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.Messages.Add(messageModel);
                    });
                }
            }
        }
    }
}
