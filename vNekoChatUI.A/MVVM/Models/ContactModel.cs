using Common.WebWpfCommon;
using Common.WPF;
using Common.WPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using vNekoChatUI.A.CustomControlEx.ListViewChatHistoryEx;
using vNekoChatUI.Character;
using ChatMessage = vNekoChatUI.Character.ChatMessage;

namespace vNekoChatUI.A.MVVM.Models
{
    internal partial class ContactModel : ObservableObject
    {
        public Action? BingBypassDetectionDoneAct { get; init; }
        public Action? SaveMessagesToPng { get; init; }

        public string ChatHistory => GetChatHistoryCallBack();

        //重要、不可修改
        private ChatClientWapper ChatClient { get; init; }

        //不可修改
        public bool IsBot { get; init; }
        public string Username { get; init; }//用于Socket客户端的名字，不可更改
        public string Usertone { get; init; }
        //public string UserborderColor { get; init; }
        //public string ImageSource { get; init; }
        public string ImageBorder { get; init; }//UI上显示的头像边框
        public Thickness ImageBorderMargin { get; init; }//UI上显示的头像边框位移
        public double ImageBorderSize { get; init; }//UI上显示的头像边框尺寸
        public ObservableCollection<MessageModel> Messages { get; init; }
        public Func<string> Say { get; init; }
        public ObservableCollection<string> Suggestion { get; init; }

        //可修改
        private MessageModel currentMessageModel;

        private CancellationTokenSource chatCts;
        private bool messageReceived;

        //可修改
        public bool IsWaitingReply => messageReceived is false;
        public bool IsHistoryChanged { get; set; }//清空聊天记录、撤回、撤回以下、编辑、重新发送、重新发送（过越狱检测）
                                                  //下层已经强制History_Changed = True了所以这个属性暂时没有作用
        public ContactModel currentCommConnact { get; set; }

        [ObservableProperty]
        private int _step;//等待消息进度

        [ObservableProperty]
        private string _profile;//写AI人设

        [ObservableProperty]
        private string _userborderColor;//气泡背景颜色

        [ObservableProperty]
        private string _signature;//Bot用于在卡片中显示最新消息，Player用于在卡片中签名

        [ObservableProperty]
        private string _displayName;//UI上显示的名字

        [ObservableProperty]
        private string _imageSource;//UI上显示的头像

        [ObservableProperty]
        private string _innerMonologue;//BING专用AI内心独白

        [ObservableProperty]
        private string _continuePrompt;//BING专用索要后续文本时的句子

        [ObservableProperty]
        private ContentControl _innerListView;//每人一个列表框


        public CharacterPlayground ChatPlayground { get; init; }

        //构造函数
        public ContactModel(CharacterPlayground _chatPlayground, string _clientName, bool _isBot, Func<int> _isDebugMode)
        {
            this.chatCts = new();
            this.messageReceived = true;

            this.IsBot = _isBot;
            this.DisplayName = _clientName;
            this.ChatClient = _chatPlayground.CreateChatClient(() => { return chatCts.Token; },
                                                               _clientName,
                                                               _isBot,
                                                               StepUpCallBack,
                                                               AutoReplyCallBack,
                                                               ReceiveMessageCallBack,
                                                               GetChatHistoryCallBack,
                                                               _isDebugMode,
                                                               GetUserProfileCallBack);
            this.ChatPlayground = _chatPlayground;//
            this.IsHistoryChanged = false;

            //
            this.Messages = new();
            this.InnerListView = new()
            {
                Content = new cListViewChatHistory()
                {
                    ItemsSource = Messages,
                    Background = new SolidColorBrush(Colors.Transparent),
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Margin = new Thickness(2),
                    AllowDrop = true,
                }
            };
            this.SaveMessagesToPng = ((cListViewChatHistory)(this.InnerListView.Content)).CaptureScreenshot;
            //
            this.Suggestion = new();
        }
    }

    //拉一下服务
    internal partial class ContactModel
    {
        IBingVisualSearchService _bingVisualSearchService = ServiceHost.Instance.GetService<IBingVisualSearchService>();
        IFlagService _flagService = ServiceHost.Instance.GetService<IFlagService>();
        IStreamService _streamService = ServiceHost.Instance.GetService<IStreamService>();
        ISignalRClientService _signalRClientService = ServiceHost.Instance.GetService<ISignalRClientService>();
    }

    //外部方法+传递到下层的回调函数
    internal partial class ContactModel
    {
        /// <summary>
        /// 将消息发送给 targetContact
        /// </summary>
        public async Task SendTo(ContactModel targetContact, string sender_said)
        {
            //收件人
            await targetContact.WaitOldTaskCancel();

            //消息路径: -> 发件人Socket客户端 -> Socket服务端 -> 收件人Socket客户端
            await this.ChatClient.SendAsync(targetContact.Username, sender_said);
        }

        /// <summary>
        /// 供viewmodel载入聊天记录
        /// </summary>
        public void LoadChatHistory(dynamic item)
        {
            if (this.IsBot)
            {
                // 历史聊天记录
                var messageModel = new MessageModel()
                {
                    Username = item.Username,
                    UsernameColor = "White",
                    UserborderColor = item.UserborderColor,
                    ImageSource = item.ImageSource,
                    Message = item.Message,
                    TokenPrice = -1,//token不用管
                    Time = item.Time,
                    IsBot = item.IsBot,
                    DisplayName = item.Username
                };

                DispatcherInvoke(() =>
                {
                    this.Messages.Add(messageModel);
                });//用Invoke同步
            }
        }

        /// <summary>
        /// 供BlazorServer覆盖聊天记录
        /// </summary>
        public void LoadChatHistoryFromBlazorServer(List<Ai_Content> list, ContactModel? player)
        {
            if (this.IsBot)
            {
                if (player is null) { return; }

                foreach (var item in list)
                {
                    if (item.Remove) { continue; }

                    MessageModel messageModel;

                    if (item.Roles == "Assistant")
                    {
                        messageModel = new MessageModel()
                        {
                            Username = this.Username,
                            UsernameColor = "White",
                            UserborderColor = this.UserborderColor,
                            ImageSource = this.ImageSource,
                            Message = item.Content,
                            TokenPrice = -1,
                            Time = DateTime.Now,
                            IsBot = true,
                            DisplayName = this.DisplayName
                        };
                    }
                    else
                    {
                        //User
                        messageModel = new MessageModel()
                        {
                            Username = player.Username,
                            UsernameColor = "White",
                            UserborderColor = player.UserborderColor,
                            ImageSource = player.ImageSource,
                            Message = item.Content,
                            TokenPrice = -1,
                            Time = DateTime.Now,
                            IsBot = false,
                            DisplayName = player.DisplayName
                        };
                    }

                    DispatcherInvoke(() =>
                    {
                        this.Messages.Add(messageModel);
                    });//用Invoke同步
                }
            }
        }

        /// <summary>
        /// 供viewmodel取消任务
        /// </summary>
        public void StopReceivingMessages()
        {
            //this.dualTest = false;

            if (chatCts.IsCancellationRequested is false)
            {
                this.chatCts.Cancel();
                _ = BlazorPushSetp(0);
            }
        }



        //用户发送了新消息所以BOT这边就停止接收旧消息，BOT这边等待一下任务结束
        private async Task WaitOldTaskCancel()
        {
            if (chatCts.IsCancellationRequested is false)
            {
                this.chatCts.Cancel();
            }
            //
            await Task.Run(async () =>
            {
                int count = 0;
                while (messageReceived is false || chatCts.IsCancellationRequested is false)
                {
                    if (++count >= 50)
                    {
                        LogProxy.Instance.Print($"can not stop receiving messages :{count}");
                    }

                    await Task.Delay(200);

                    if (count >= 50 * 6 * 5)
                    {

                        LogProxy.Instance.Print($"task exit");//
                        break;
                    }
                }
            });

            this.chatCts = new();
            this.messageReceived = false;
            this.Step = 0;
        }



        //接收下层AI的回复
        private void AutoReplyCallBack(string response)
        {
            if (chatCts.IsCancellationRequested)
            {
                LogProxy.Instance.Print($"已停止接收消息");
                this.messageReceived = true;
                this.Step = 0;
                return;
            }
            this.messageReceived = true;

            if (this.IsBot)
            {
                var chatMessage = new ChatMessage().GetJsonObject(response);

                //BOT的自动回复，写入聊天记
                {
                    var token = chatMessage.TotalTokens;
                    var message = chatMessage.ReceiverMessage;

                    //于DebugMode时，下层不会返回ChatGPT的回复，故此处使用随机句子作自动回复
                    if (string.IsNullOrWhiteSpace(message)) { message = $"DebugMode: {this.Say.Invoke()}"; }

                    //var messageModel = new MessageModel()
                    //{
                    //    Username = this.Username,
                    //    UsernameColor = "White",
                    //    UserborderColor = this.UserborderColor,
                    //    ImageSource = this.ImageSource,
                    //    Message = message,
                    //    TokenPrice = token,//本次答复消耗token
                    //    Time = DateTime.Now,
                    //    IsBot = this.IsBot,
                    //    DisplayName = this.DisplayName,
                    //};

                    ////ObservableCollection<MessageModel> Messages 对象已绑定到UI，
                    ////故在其他线程调用此处的回调函数时，只能委托UI线程去操作
                    //System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                    //{
                    //    this.Messages.Add(messageModel);
                    //});

                    this.Signature = message.Substring(0, Math.Min(message.Length, 20));

                    this.currentMessageModel.Message = message;
                    this.currentMessageModel.TokenPrice = token;
                    this.currentMessageModel.Time = DateTime.Now;


                    DispatcherBeginInvoke(async () =>
                    {
                        //保险起见，好像是Debug模式才会走这里
                        if (this.currentMessageModel.StartStreaming is false)
                        {
                            this.Messages.Add(currentMessageModel);//用BeginInvoke异步
                        }

                        await BlazorPushChatHistory();  //传递到Blazor那边试试

                        this.UseBingVisualSearch = false;//在这里复位该flag，使下一次对话不传递图像地址
                    });



                    //写入发件方聊天记录
                    //System.Windows.Application.Current.Dispatcher.BeginInvoke(async () =>
                    //{
                    //    if (this.dualTest) 
                    //    {
                    //        await Task.Delay(100);
                    //        await this.SendTo(currentCommConnact, $"{currentMessageModel.Message}");
                    //    }
                    //});
                }
            }
        }

        //供下层查询完整聊天记录
        private string GetChatHistoryCallBack()
        {
            if (this.IsBot is false) { return "I am Player"; }

            var message_history = this.Messages.Select(m => new
            {
                roles = (m.IsBot ? "Assistant" : "User"),
                content = m.Message
            });
            var data_content = new
            {
                ai_name = this.Username,
                ai_profile = this.Profile.Trim(),//string.Format(this.Profile.Trim(), this.Username, this.Usertone),
                ai_content = message_history,
                ai_innermonologue = this.InnerMonologue,
                ai_continuePrompt = this.ContinuePrompt,
                history_changed = this.IsHistoryChanged,
                bypass_detection = _flagService.TryUseBingBypassDetection[0],//本次请求是否启用过越狱检测
            };

            var json = JsonSerializer.Serialize(data_content, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            this.IsHistoryChanged = false;//使用完毕，复位
            //this.BingBypassDetectionDoneAct?.Invoke();//使用完毕，复位
            return json;
        }

        //供下层查询用户资料
        private SenderInfo GetUserProfileCallBack()
        {
            return new SenderInfo(this.Username,
                                  this.UserborderColor,
                                  this.ImageSource,
                                  this.IsBot,
                                  this.DisplayName);
        }

        //供下层添加消息至聊天记录（接收他人消息，以及创建Bing流式传输用的回调函数）
        private void ReceiveMessageCallBack(string response)
        {
            if (this.IsBot)
            {
                ChatMessage chatMessage = new ChatMessage().GetJsonObject(response);

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
                        DisplayName = sender.DisplayName,
                    };

                    DispatcherInvoke(async () =>
                    {
                        if (this.Username != sender.UserName)
                        {
                            messageModel.IsBot = false;
                        }
                        this.Messages.Add(messageModel);//用Invoke同步
                        await BlazorPushChatHistory();  //传递到Blazor那边试试
                    });
                }

                //流式传输测试
                {
                    currentMessageModel = new MessageModel()
                    {
                        Username = this.Username,
                        UsernameColor = "White",
                        UserborderColor = this.UserborderColor,
                        ImageSource = this.ImageSource,
                        Message = "...",
                        TokenPrice = -1,
                        Time = DateTime.Now,
                        IsBot = this.IsBot,
                        DisplayName = this.DisplayName,
                        StartStreaming = false,
                    };

                    //Bing流式传输
                    _streamService.RegisterStremingText(this.Username, (x) =>
                    {
                        DispatcherInvoke(async () =>
                        {
                            var flag = this.currentMessageModel.StartStreaming is false;
                            if (this.currentMessageModel.StartStreaming is false)
                            {
                                this.currentMessageModel.StartStreaming = true;
                                this.Messages.Add(currentMessageModel);
                            }

                            //用→( )←包裹起来代表额外信息，下层读消息写入上下文时会砍掉额外信息
                            var text = $"{x}\n\n→(\nMessage not fully processed yet.\n)←";
                            this.currentMessageModel.Message = text;
                            this.currentMessageModel.Time = DateTime.Now;

                            var temp = x.Substring(0, Math.Min(x.Length, 20)).Replace("\n", "").Replace("\r", "");
                            if (this.Signature != temp) { this.Signature = temp; }

                            await BlazorStartStreaming(text, flag);
                        });
                    });
                    //Bing推荐回复
                    _streamService.RegisterSuggestion(this.Username, (sug) =>
                    {
                        DispatcherInvoke(() =>
                        {
                            this.Suggestion.Clear();
                            sug?.ForEach(x => this.Suggestion.Add(x));
                        });
                    });
                }
            }
        }
        //步骤指示（因为下层会各种报错重试，所以不能用叠加的形式搞，只能用赋值）
        private void StepUpCallBack(int step)
        {
            this.Step = step;
            _ = BlazorPushSetp(step);
        }
    }

    //内部方法
    internal partial class ContactModel
    {
        private void DispatcherInvoke(Action act)
        {
            //用Invoke同步
            System.Windows.Application.Current.Dispatcher.Invoke(act);
        }
        private void DispatcherBeginInvoke(Action act)
        {
            //用BeginInvoke异步
            System.Windows.Application.Current.Dispatcher.BeginInvoke(act);
        }

        //本来GetChatHistoryCallBack()应该是给下层调用的，这里省事顺便用一下
        private async Task BlazorPushChatHistory()
        {
            await _signalRClientService.PushChatHistory(ChatHistory);
        }
        private async Task BlazorStartStreaming(string text, bool once)
        {
            if (once)
            {
                await _signalRClientService.PushChatHistory(ChatHistory);
            }
            await _signalRClientService.StartStreaming(text);
        }
        private async Task BlazorPushSetp(int step)
        {
            await _signalRClientService.PushStep($"{step}");
        }
    }

    //bing上传图片相关
    internal partial class ContactModel
    {
        public string? BingVisualSearchImageUrl => _bingVisualSearchService.ImageUrl;

        [ObservableProperty]
        private bool useBingVisualSearch;
        //public bool UseBingVisualSearch
        //{
        //    get { return _flagService.TryUseBingVisualSearch; }
        //    set
        //    {
        //        _flagService.TryUseBingVisualSearch = value;
        //        NotifyPropertyChanged();
        //        NotifyPropertyChanged(nameof(BingVisualSearchImageUrl));
        //    }
        //}

        [RelayCommand]
        private async Task OnBingVisualSearchAsync()
        {
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new())
            {
                openFileDialog.Title = "选择文件";
                openFileDialog.Filter = "所有文件 (*.*)|*.*";
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    var response = await _bingVisualSearchService.UploadImageAsync(@$"{selectedFilePath}");
                    var res_flag = response.Item1;
                    var res_text = response.Item2;
                    LogProxy.Instance.Print($"ImageUrl = {res_text}");

                    this.UseBingVisualSearch = res_flag;
                }
                else
                {
                    this.UseBingVisualSearch = false;
                }
            }
        }
    }
}
