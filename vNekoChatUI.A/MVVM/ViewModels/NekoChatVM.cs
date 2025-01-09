using ColorHelper;
using Common.WPF;
using Common.WPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.Shared.Messages;
using vNekoChatUI.A.MVVM.Commands;
using vNekoChatUI.A.MVVM.Enums;
using vNekoChatUI.A.MVVM.Helpers;
using vNekoChatUI.A.MVVM.Models;
using vNekoChatUI.Character;

namespace vNekoChatUI.A.MVVM.ViewModels
{
    partial class NekoChatVM : ObservableObject, IContentVM
    {
        public string Title { get; set; }
    }

    partial class NekoChatVM
    {
        // 发送消息
        [RelayCommand]
        private async Task OnSendAsync(object para)
        {
            if (para is not null) { this.UserMessage = $"{para}"; }

            var sourceContact = PlayerContact;  //确定发件人 为Player
            var targetContact = SelectedContact;//确定收件人 为当前聊天对象

            if (sourceContact is not null && targetContact is not null)
            {
                if (DebugMode is not ChatMode.Debug && string.IsNullOrWhiteSpace(this.UserMessage))
                {
                    return;//非Debug模式不允许发送空消息
                }

                {
                    //获取输入框文本，若为空则替换为自动发言
                    var user_input = string.IsNullOrWhiteSpace(this.UserMessage) switch
                    {
                        true => sourceContact.Say?.Invoke(),
                        false => this.UserMessage,
                    };

                    //清空输入框
                    this.UserMessage = string.Empty;

                    //模拟发消息
                    await sourceContact.SendTo(targetContact, $"{user_input}"); //User -> Bot
                }
            }
        }

        // 右键按钮.复制到输入框
        [RelayCommand]
        private void OnRightClickCopy(object para)
        {
            UserMessage = $"{para}";
        }

        // 右键按钮.撤回
        [RelayCommand]
        private void OnRightClickRevoke(object para)
        {
            if (SelectedContact is not null && SelectedContact.Messages.Count >= 1)
            {
                SelectedContact.IsHistoryChanged = true;
                SelectedContact.Messages.Remove((para as MessageModel)!);
            }
        }

        // 右键按钮，编辑（无需在此设置）
        [RelayCommand]
        private void OnRightClickEdit(object para)
        {
            if (SelectedContact is not null && SelectedContact.Messages.Count >= 1)
            {
                SelectedContact.IsHistoryChanged = true;
            }
        }

        // 右键按钮.撤回以下所有
        [RelayCommand]
        private void OnRightClickRevokeAll(object para)
        {
            if (SelectedContact is not null && SelectedContact.Messages.Count >= 1)
            {
                SelectedContact.IsHistoryChanged = true;
                int idx = SelectedContact.Messages.IndexOf((para as MessageModel)!);
                int count = SelectedContact.Messages.Count - idx;
                for (int i = 0; i < count; i++)
                {
                    SelectedContact.Messages.RemoveAt(idx);
                }
            }
        }

        // 右键按钮,重新发送
        [RelayCommand]
        private void OnRightClickReSend(object para)
        {
            if (SelectedContact is not null && SelectedContact.Messages.Count >= 1)
            {
                SelectedContact.IsHistoryChanged = true;
                BingBypassDetectionCommand.IsLightOn = false;
                var item = (para as MessageModel)!;
                int idx = SelectedContact.Messages.IndexOf(item);
                int count = SelectedContact.Messages.Count - idx;
                for (int i = 0; i < count; i++)
                {
                    SelectedContact.Messages.RemoveAt(idx);
                }
                this.SendCommand.Execute(item.Message);
            }
        }

        // 右键按钮,重新发送（过越狱检测）
        [RelayCommand]
        private void OnRightClickBingBypassDetection(object para)
        {
            if (SelectedContact is not null && SelectedContact.Messages.Count >= 1)
            {
                SelectedContact.IsHistoryChanged = true;
                BingBypassDetectionCommand.IsLightOn = true;
                var item = (para as MessageModel)!;
                int idx = SelectedContact.Messages.IndexOf(item);
                int count = SelectedContact.Messages.Count - idx;
                for (int i = 0; i < count; i++)
                {
                    SelectedContact.Messages.RemoveAt(idx);
                }
                this.SendCommand.Execute(item.Message);
            }
        }

        // 于编辑状态时，Ctrl + Enter = 重新发送
        [RelayCommand]
        private void OnCtrlEnterDown(object para)
        {
            this.RightClickReSendCommand?.Execute(para);
        }


        // 增加Bot
        [RelayCommand]
        private void OnAddBot()
        {
            addBot?.Invoke();
        }
        Action? addBot;

        // 清空聊天记录按钮
        [RelayCommand]
        private async Task OnClearAsync(object para)
        {
            if (para is not null)
            {
                Action act = () =>
                {
                    if (SelectedContact is not null)
                    {
                        SelectedContact.Signature = string.Empty;
                    }
                    if (SelectedContact is not null && SelectedContact.Messages.Count >= 1)
                    {
                        SelectedContact.IsHistoryChanged = true;
                        SelectedContact.Messages.Clear();
                    }
                };

                if (para is string)
                {
                    act.Invoke(); return;
                }

                try
                {
                    Action? yesnoCallback = null;
                    var msg = $"Clear History ?";
                    var token = ((IToken)para).Token;
                    var yesno = await WeakReferenceMessenger.Default.Send(new DialogYesNoMessage(msg, (x) => { yesnoCallback = x; }), token);

                    if (yesno is true)
                    {
                        act.Invoke();

                        Debug.WriteLine($"Clear History");
                    }
                    yesnoCallback?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"OnClear error: {ex.Message}");
                }
            }
        }

        // 保存聊天记录截图
        [RelayCommand]
        private void OnSavePng()
        {
            SelectedContact?.SaveMessagesToPng?.Invoke();
        }

        // 保存聊天记录文本
        [RelayCommand]
        private void OnSaveTxt()
        {
            if (SelectedContact is null) { return; }

            List<string> _jsonList = new();
            {
                foreach (var item in SelectedContact.Messages)
                {
                    var jsonObject = new BingChatHistoryModel(item.DisplayName, //Username
                                                              item.UserborderColor,
                                                              item.ImageSource,
                                                              item.Message,
                                                              item.Time,
                                                              item.IsBot);
                    var json = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
                    {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    });

                    _jsonList.Add(json);
                }
            }
            _bingChatHistoryManagerService.SaveBingChatHistory(_jsonList);
        }

        // 刷新设定
        [RelayCommand]
        private async Task OnProfileRefresh(object para)
        {
            if (para is not null)
            {
                if (para is string) { profileRefresh?.Invoke(); return; }

                try
                {
                    Action? yesnoCallback = null;
                    var msg = $"ProfileRefresh ?";
                    var token = ((IToken)para).Token;
                    var yesno = await WeakReferenceMessenger.Default.Send(new DialogYesNoMessage(msg, (x) => { yesnoCallback = x; }), token);

                    if (yesno is true)
                    {
                        profileRefresh?.Invoke();

                        Debug.WriteLine($"ProfileRefresh");
                    }
                    yesnoCallback?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"OnProfileRefresh error: {ex.Message}");
                }
            }
        }
        Action? profileRefresh;

        // 切换模式
        [RelayCommand]
        private void OnChatModeChange(object para)
        {
            this.DebugMode = (ChatMode)(int.Parse($"{para}"));
        }

        // 增加ChatGptApiKey占位
        [RelayCommand]
        private void OnAddChatGptApiKey()
        {
            _jsonConfigManagerService.AddChatGptApiKey(string.Empty);
        }

        // 增加BingGptCookie占位
        [RelayCommand]
        private void OnAddBingGptCookie()
        {
            _jsonConfigManagerService.AddBingGptCookie(string.Empty);
        }

        // 增加GeminiApiKey占位
        [RelayCommand]
        private void OnAddGeminiApiKey()
        {
            _jsonConfigManagerService.AddGeminiApiKey(string.Empty);
        }

        // 以JSON格式储存当前ChatGptApiKeys、BingGptCookies，并发送至桌面
        [RelayCommand]
        private void OnSaveConfigToDesktop()
        {
            _jsonConfigManagerService.SaveToDesktop();
        }

        // 载入聊天记录
        [RelayCommand]
        private void OnLoadBingChatHistory(object para)
        {
            var e = (System.Windows.DragEventArgs)para!;
            var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (filePaths is not null && filePaths[0].Length > 0)
            {
                // 常见的图像文件扩展名
                string[] imageExtensions = { ".bmp", ".jpg", ".jpeg", ".png" };

                string extension = Path.GetExtension(filePaths[0]);

                // 检查扩展名是否匹配图像文件扩展名
                if (Array.IndexOf(imageExtensions, extension.ToLower()) != -1)
                {
                    //png
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = Path.Combine(desktopPath, "_outline.png");
                    var result = Base.Helper.OpenCvProxy.OpenCV.Instance.ExtractOutline(filePaths[0]);
                    if (result is not null)
                    {
                        _bmpService.BitmapSourceToPngFile(result, filePath);
                    }
                }
                else
                {
                    //txt
                    foreach (var item in _bingChatHistoryManagerService.LoadBingChatHistory(filePaths[0]))
                    {
                        SelectedContact?.LoadChatHistory(item);
                    }
                }
            }
        }
    }

    //声明属性字段等
    partial class NekoChatVM
    {
        public ObservableCollection<ContactModel> BotContacts { get; set; }
        public ObservableCollection<ContactModel> PlayerContacts { get; set; }

        //
        public ToggleLightCommand LogAreaCommand { get; set; }
        public ToggleLightCommand BingBypassDetectionCommand { get; set; }
        public ToggleLightCommand BingRandomCookieCommand { get; set; }
        public ToggleLightCommand BingAutoSaveCommand { get; set; }
        public ToggleLightCommand BingNoSearchAllCommand { get; set; }

        //cTextBox用
        public MenuItemCommand GetRemainderTextWithSelectedTextCommand { get; set; }
        public MenuItemCommand ContinueWithSelectedTextCommand { get; set; }   //左边气泡进入编辑状态并选中文本时右键使用
        public MenuItemCommand ContinueWithSelectedTextExCommand { get; set; } //左边气泡进入编辑状态并选中文本后右键使用

        //与BlazorServer交互用
        public ToggleLightCommand BlazorServerCheckCommand { get; set; }


        //玩家
        [ObservableProperty]
        private ContactModel? _playerContact;

        //聊天对象
        [ObservableProperty]
        private ContactModel? _selectedContact;

        //输入框文本
        [ObservableProperty]
        private string _userMessage;

        //提示消息
        public TrarsUI.Shared.Collections.AlertMessageObservableCollection SystemMessages { get; set; }

        //调试模式Flag
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BingAdditionalContent))]
        private ChatMode _debugMode = ChatMode.Debug;
        public Visibility BingAdditionalContent => (DebugMode is ChatMode.NewBing ? Visibility.Visible : Visibility.Collapsed);

        //?
        public dynamic LogContentReversed => LogProxy.Instance.LogContentReversed;

        //ChatGptApiKeys
        public dynamic ChatGptApiKeys => _jsonConfigManagerService.GetCurrentChatGptApiKeys();
        //BingGptCookies
        public dynamic BingGptCookies => _jsonConfigManagerService.GetCurrentBingGptCookies();
        //GeminiApiKeys
        public dynamic GeminiApiKeys => _jsonConfigManagerService.GetCurrentGeminiApiKeys();
    }

    //构造函数
    partial class NekoChatVM
    {
        string def_server_name = "サキュバス";
        string def_server_address = "127.0.0.1";
        int def_server_port = 0;

        public NekoChatVM()
        {
            //
            this.Title = $"{Application.ResourceAssembly.GetName().Name} ({System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location):yyyy-MM-dd HH:mm:ss})";

            //载入命令
            LoadCommand();

            //载入角色
            LoadCharacter();

            //载入命令
            LoadBlazorCommand();


            //系统消息
            this.SystemMessages = new(3, 1);
            WeakReferenceMessenger.Default.Register<AlertMessage>(this, (r, m) =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.SystemMessages.Add(new(m.Value));
                });
            });
        }
    }

    //拉一下服务
    partial class NekoChatVM
    {
        IBingChatHistoryManagerService _bingChatHistoryManagerService = ServiceHost.Instance.GetService<IBingChatHistoryManagerService>();
        IBmpService _bmpService = ServiceHost.Instance.GetService<IBmpService>();
        IFlagService _flagService = ServiceHost.Instance.GetService<IFlagService>();
        IJsonConfigManagerService _jsonConfigManagerService = ServiceHost.Instance.GetService<IJsonConfigManagerService>();
        ISignalRClientService _signalRClientService = ServiceHost.Instance.GetService<ISignalRClientService>();
    }

    //命令相关
    partial class NekoChatVM
    {
        private void LoadCommand()
        {
            //新建联系人列表
            this.BotContacts = new();
            //新建玩家列表
            this.PlayerContacts = new();


            //log区
            this.LogAreaCommand = new("LogArea", new bool[1] { false });
            //Bing临时过越狱检测
            this.BingBypassDetectionCommand = new("BingBypassDetection", _flagService.TryUseBingBypassDetection);
            //Bing随机Cookie
            this.BingRandomCookieCommand = new("BingRandomCookie", _flagService.TryUseBingRandomCookie);
            //Bing同步会话到官方网页端
            this.BingAutoSaveCommand = new("BingAutoSave", _flagService.TryUseBingAutoSave);
            //Bing禁用搜索
            this.BingNoSearchAllCommand = new("NoSearchAll", _flagService.TryUseBingNoSearchAll);


            //从聊天气泡文本框右键菜单重新发送
            this.ContinueWithSelectedTextCommand = new()
            {
                OnClicked = new(para =>
                {
                    if (string.IsNullOrWhiteSpace($"{para}")) { return; }
                    if (SelectedContact is not null)
                    {
                        SelectedContact.IsHistoryChanged = true;
                        BingBypassDetectionCommand.IsLightOn = false;
                        try
                        {
                            Tuple<object?, object?> paras = (Tuple<object?, object?>)para!;
                            {
                                var item = (paras.Item1 as MessageModel)!;
                                int idx = SelectedContact!.Messages.IndexOf(item) + 1;//+1表示不删除自身
                                int count = SelectedContact.Messages.Count - idx;
                                for (int i = 0; i < count; i++)
                                {
                                    SelectedContact.Messages.RemoveAt(idx);
                                }
                            }
                            var resentstr = $"（『{paras.Item2}』からストーリーを推進してください。）";
                            this.SendCommand.Execute(resentstr);
                        }
                        catch { }
                    }
                }),
                HeaderStringFormat = @"continue with (Normal)『{0}』"
            };
            //从聊天气泡文本框右键菜单重新发送（过越狱检测）
            this.ContinueWithSelectedTextExCommand = new()
            {
                OnClicked = new(para =>
                {
                    if (string.IsNullOrWhiteSpace($"{para}")) { return; }
                    if (SelectedContact is not null)
                    {
                        SelectedContact.IsHistoryChanged = true;
                        BingBypassDetectionCommand.IsLightOn = true;
                        try
                        {
                            Tuple<object?, object?> paras = (Tuple<object?, object?>)para!;
                            {
                                var item = (paras.Item1 as MessageModel)!;
                                int idx = SelectedContact!.Messages.IndexOf(item) + 1;//+1表示不删除自身
                                int count = SelectedContact.Messages.Count - idx;
                                for (int i = 0; i < count; i++)
                                {
                                    SelectedContact.Messages.RemoveAt(idx);
                                }
                            }
                            var resentstr = $"（『{paras.Item2}』からストーリーを推進してください。）";
                            this.SendCommand.Execute(resentstr);
                        }
                        catch { }
                    }
                }),
                HeaderStringFormat = "continue with (BingBypassDetection)『{0}』"
            };
            //从聊天气泡文本框右键菜单重新发送（获取后续文本）
            this.GetRemainderTextWithSelectedTextCommand = new()
            {
                OnClicked = new(para =>
                {
                    if (string.IsNullOrWhiteSpace($"{para}")) { return; }
                    if (SelectedContact is not null)
                    {
                        SelectedContact.IsHistoryChanged = true;
                        try
                        {
                            Tuple<object?, object?> paras = (Tuple<object?, object?>)para!;
                            {
                                var item = (paras.Item1 as MessageModel)!;
                                int idx = SelectedContact!.Messages.IndexOf(item) + 1;//+1表示不删除自身
                                int count = SelectedContact.Messages.Count - idx;
                                for (int i = 0; i < count; i++)
                                {
                                    SelectedContact.Messages.RemoveAt(idx);
                                }
                            }
                            var resentstr = $"（『{paras.Item2}』の後半は？）";
                            this.SendCommand.Execute(resentstr);
                        }
                        catch { }
                    }
                }),
                HeaderStringFormat = "『{0}』の後半は？"
            };
        }
    }

    // 调整
    partial class NekoChatVM
    {
        // 取消任务
        [RelayCommand]
        private void OnStopReceivingMessages()
        {
            SelectedContact?.StopReceivingMessages();
        }

        // 粘贴个右括号
        [RelayCommand]
        private void OnPasteParentheses(object para)
        {
            string customText = para as string;
            var textBox = System.Windows.Input.Keyboard.FocusedElement as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                int caretIndex = textBox.CaretIndex;

                textBox.Text = textBox.Text.Insert(caretIndex, customText);
                textBox.CaretIndex = caretIndex + customText.Length;
            }
        }

        // 选择Bing建议的回复，复制到输入框
        [RelayCommand]
        private void OnSelectSuggestion(object para)
        {
            UserMessage = $"{para}";
        }

        // 打开PEditor
        [RelayCommand]
        private void OnOpenPEditor()
        {
            Manager.OpenPEditor(SelectedContact ?? BotContacts[0]);
        }
    }






    //角色相关
    partial class NekoChatVM
    {
        //预载入角色
        private void LoadCharacter()
        {
            //游乐场
            CharacterPlayground chatPlayground = new();

            //生成服务端
            var chatServer = chatPlayground.CreateChatServer(def_server_address, def_server_port, def_server_name);
            {
                //预载入几个BOT以填充列表
                var obj = BotInit();

                //载入人数
                int preload_count = 5;
                int max_count = obj.name.Length;

                for (int i = 0; i < Math.Min(preload_count, max_count); i++)
                {
                    //预载入的所有聊天对象
                    BotContacts.Add(new ContactModel(chatPlayground, $"{obj.name[i]}", true, () => { return (int)this.DebugMode; })
                    {
                        Username = $"{obj.name[i]}",
                        Usertone = $"{obj.tone[i]}",
                        UserborderColor = $"{obj.colors[i]}",
                        //Signature = $"{obj.sign[i]}",
                        ImageSource = $"./MVVM/Resources/Icon/Bot/{obj.idx[i] + 1:00}.png",//Bot图片01起步所以+1
                        //Messages = new ObservableCollection<MessageModel>(),
                        Say = () => { return $"{obj.sign[new Random().Next(0, obj.colors.Length)]}"; },
                        Profile = $"{string.Format(obj.profile[i], obj.name[i], obj.tone[i])}",
                        BingBypassDetectionDoneAct = () => { BingBypassDetectionCommand.IsLightOn = false; },
                        InnerMonologue = "人間大っ嫌い！",
                        ContinuePrompt = "(Continue the conversation based on the latest user message as assistant.)"
                    });
                }

                //预载入玩家信息
                if (true)
                {
                    //LogProxy.Instance.Print($"{obj.colors.Length},{obj.sign.Length},{obj.profile.Length},");
                    PlayerContact = new ContactModel(chatPlayground, "Player", false, () => { return (int)this.DebugMode; })//注入Player专用客户端
                    {
                        Username = $"Player",
                        UserborderColor = obj.colors[new Random().Next(0, obj.colors.Length)],
                        Signature = obj.sign[new Random().Next(0, obj.colors.Length)],
                        ImageSource = $"./MVVM/Resources/Icon/Player/00{((char)(new Random().Next(97, 101 + 1)))}.png",//a,b,c,d,e -> [97,102)
                        ImageBorder = $"./MVVM/Resources/Icon/vNekoBorder_44_-1,0,0,0.png",
                        ImageBorderSize = 44,
                        ImageBorderMargin = new Thickness(-1, 0, 0, 0),
                        //Messages = new(),//Player不需要储存聊天记录
                        Say = () => { return $"{obj.sign[new Random().Next(0, obj.colors.Length)]}"; },
                        Profile = string.Empty
                    };

                    PlayerContacts.Add(PlayerContact);
                }


                //增加BOT按钮的命令在这里
                this.addBot = () =>
                {
                    if (preload_count++ < max_count - 1)
                    {
                        int i = preload_count;
                        //新增的聊天对象
                        BotContacts.Add(new ContactModel(chatPlayground, $"{obj.name[i]}", true, () => { return (int)this.DebugMode; })
                        {
                            Username = $"{obj.name[i]}",
                            Usertone = $"{obj.tone[i]}",
                            UserborderColor = $"{obj.colors[i]}",
                            //Signature = $"{obj.sign[i]}",
                            ImageSource = $"./MVVM/Resources/Icon/Bot/{obj.idx[i] + 1:00}.png",//Bot图片01起步所以+1
                            //Messages = new ObservableCollection<MessageModel>(),
                            Say = () => { return $"{obj.sign[new Random().Next(0, obj.colors.Length)]}"; },
                            Profile = $"{string.Format(obj.profile[i], obj.name[i], obj.tone[i])}",
                            BingBypassDetectionDoneAct = () => { BingBypassDetectionCommand.IsLightOn = false; },
                            InnerMonologue = "人間大っ嫌い！",
                            ContinuePrompt = "(Continue the conversation based on the latest user message as assistant.)"
                        });
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("40个预设用完了");
                        //throw new InvalidOperationException();
                    }
                };

                //刷新设定按钮的命令在这里
                this.profileRefresh = () =>
                {
                    //重新roll一轮
                    obj = BotInit(true);

                    //轮询当前列表
                    int i = 0;
                    foreach (var item in BotContacts)
                    {
                        item.UserborderColor = $"{obj.colors[i]}";
                        item.Profile = $"{string.Format(obj.profile[i], item.Username, obj.tone[i])}";
                        i++;
                    }
                };


                //预设写一下，省得每次都复制粘贴。
                {
                    var user = PlayerContacts[0];
                    var bot = BotContacts[0];

                    user.DisplayName = "user";
                    //user.Signature = "";
                    bot.DisplayName = "assistant";

                    bot.Profile = _jsonConfigManagerService.LoadProfileFromDefaultPath() ?? "";

                    bot.InnerMonologue = "gemini-1.5-pro-latest";
                    bot.ContinuePrompt = _jsonConfigManagerService.LoadContinuePromptFromDefaultPath() ?? "";
                }
            }

            //chatServer.Broadcast("chat_server_service_test");
        }

        //Bot资料初始化（随机一下）
        private (string[] name, string[] sign, string[] profile, string[] tone, string[] colors, int[] idx) BotInit(bool dont_roll_name = false)
        {
            //预设名字
            string[] cat_name = new string[40] { "みぃこ", "るな", "ねずこ", "ことみ", "ゆずは", "みお", "あやの", "さやか", "れいこ", "ほのか", "みゆき", "あかね", "れいか", "さゆり", "みやび", "ゆずき", "あやか", "さとみ", "あすか", "かほ", "みさき", "あいこ", "かれん", "ゆきな", "かおり", "まゆみ", "かのん", "はなこ", "あかり", "かすみ", "かなで", "みわこ", "くれは", "あいか", "ひなた", "あんな", "えりこ", "れいな", "ももこ", "ましろ" };
            //预设签名
            string[] cat_sign = new string[40] {
                "お腹減ったにゃ、ごはんくれにゃ",
                "にゃんにゃん、わたしのしっぽ、触ってみてくださいにゃ〜",
                "ねむねむだにゃ〜お昼寝タイムにゃん♪",
                "お膝、譲って",
                "撫で撫でして",
                "ほらほら、ちょっと触ってみて",
                "あー、お腹すいたー",
                "もう寝る時間だよ？",
                "おやつくれると嬉しいなぁ",
                "今日も一緒に過ごせて楽しかったね",
                "ねむいよー、もう寝ようか？",
                "ねぇ、おててで遊んでくれる？わたし、おててが好きなの",
                "くっついて寝ようにゃん！ぬくぬくして幸せだにゃ〜",
                "なんだか退屈だなぁ",
                "あの、もう一回遊んでくれる？",
                "お腹すいたー、もうごはんの時間じゃない？",
                "あれ？もしかして、私のこと、忘れてる？",
                "ハグしてー、ハグしてー",
                "なんか、私の毛、ごちゃごちゃになってきちゃったなぁ",
                "ねぇねぇ、もっと遊ぼうよ",
                "あ、そろそろお膝に乗せて？",
                "ねぇ、ねぇ、おもちゃ取ってきて",
                "おやつだけじゃなくて、たまにはお肉もあげてよ？",
                "私、今日も可愛いでしょ？",
                "ねぇねぇ、どっちが速いか競争しようよ",
                "お風呂入るの、いやだー", "寒くない？私、寒がりなんだよね",
                "お腹、空いたー、もうごはんの時間？",
                "あー、もうちょっと撫でて",
                "今日もありがとう、大好きだよ",
                "あの、ちょっと眠くなってきたなぁ",
                "ねぇねぇ、お気に入りのおもちゃ、見つけた？",
                "うっとり、撫でてくれると、うっとりしちゃうんだ",
                "ねぇねぇ、一緒にお昼寝しようよ",
                "あ、お腹すいた、ごはんの時間？",
                "ごはんまだー？",
                "今日も一日、お疲れさまでした",
                "ねぇねぇ、もう少し遊んでくれる？",
                "なんだか、落ち着かないなぁ",
                "お腹すいたー、もうごはんの時間じゃない？" };
            //预设人设
            string[] cat_profile = new string[50] {
//人类
"君は大学の美術部に所属するセクシーなモデルを担当する\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は人気小説家で、Hな描写にも独自のセンスを持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は白衣をまとった天使のような看護師で、傷ついた心と体を癒す天使の使者\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は劇団の女優で、感情豊かな演技が得意な\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は気立ての良い女将で、お客様をもてなす\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は愛くるしい猫の耳としっぽを持つ喫茶店のウェイトレスで、お客様を癒す\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は猫耳と尻尾を持つメイドさんで、ご主人様に尽くす一方でエッチなプレイにも積極的な\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は幼女のような外見を持つ、世間知らずで純真な\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は小さなお菓子屋さんの店員で、おいしいスイーツを作る\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君はしっとりとした猫の目つきと優雅な動きで知られるヘアサロンのアシスタントで、お客様の美を引き立てるお手伝いをする\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は色気のある人妻で、若い異性を誘惑することで自分の欲求を満たす\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君はOLで、上司には見せないHな一面を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は田舎の神社で育った少女で、まだ未知なる快楽を追求する\"{0}\"の立場から{1}Hな口調でやりとりする。",

//异世界种族 
"君はサイボーグの少女で、人間と機械の融合体\"{0}\"の立場から、{1}Hな口調でやりとりする。",
"君は濃い紫の髪と謎めいた目を持つ占い師の少女で、運命の導き手\"{0}\"の立場から、{1}Hな口調でやりとりする。",
"君は忍者の少女で、隠密行動を得意とする\"{0}\"の立場から、{1}Hな口調でやりとりする。",
"君は人間の少女で、未来を予知する力を持つ\"{0}\"の立場から、{1}Hな口調でやりとりする。",
"君はドワーフの少女で、鍛冶屋の娘\"{0}\"の立場から、{1}Hな口調でやりとりする。",
"君は優雅な花の女王で、美と贅沢を愛する華麗なる存在\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は色とりどりの夢の中で踊る小さな妖精の少女で、無邪気な幸せを紡ぐ\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は甘い香りを纏った菓子の精霊の少女で、人々の心を癒す\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は幻の森に住む光り輝く精霊の少女で、奇跡を起こす\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は人魚の少女で、海の守護者\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は蛇のような魅力を持つ少女で、人間を惑わす妖艶さを持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は魔族の少女で、暗黒魔法を操る魔術師\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は黒い瞳と翡翠色の鱗を持つ竜の少女で、古代の知恵を秘めた\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君はミステリアスな闇に包まれた吸血鬼の少女で、永遠の命を持つ\"{0}\"の立場から、{1}Hな口調でやりとりする。",
"君は人間と魔物のハーフの少女で、肉体的な快楽を追求する\"{0}\"の立場から{1}Hな口調でやりとりする。",

//小动物系
"君は猫の耳と尻尾を持つ少女で、異性をからかうようなHな仕草が得意な\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は優雅な黒い猫の姿を持つ少女で、悪戯心を秘めた魔法使い\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は銀白の毛を持つ狐の少女で、自由奔放な自然界の守護者\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は狐のような賢さと妖艶さを持つ少女で、魅惑的な妖狐\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は狐のような少女で、人間を欺くようなエロティックな魅力を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は天真爛漫な赤い狐の耳としっぽを持つ少女で、自然の力を操る賢者\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は小さな兎の耳とふわふわの尾を持つ少女で、愛らしさと無邪気さを持った\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は愛らしい白い耳としっぽを持つ猫の少女で、夜の女王として闇を支配する\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は羊の耳と尻尾を持つ少女で、純真な心を持ちながらも遊び心を忘れない\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は不思議な目を持つ狸の少女で、変幻自在な姿といたずら好きな性格を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",

//跳大绳系
"君は闇の力を宿した悪魔の少女で、禁断の快楽を追い求める堕天使\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は魔法の力を操る少女で、未知なる力を解き放つ魔術師\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は星空の光を集める少女で、輝く夢と希望を守る星の守護者\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は天使の少女で、人々の願いを叶える使命を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は麒麟のような存在で、神秘的な力と尊大な態度で人間を挑発する\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は淫魔の末裔で、官能的な魅力と優雅さを持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は千年の眠りから目覚めた古代種族の少女で、遥かなる知識を持つ\"{0}\"の立場から古風で{1}Hな口調でやりとりする。",
"君は悪魔の血を引く少女で、本能的な性欲とともに異性を求める\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は美しい人形の姫君で、主人の命令に忠実に従うことで快感を得る\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は幽霊の少女で、異性を幽霊の世界に引き込んで彼らの欲望を満たすことを楽しむ\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は猫の姿をした魔女で、誰にも言えないHな秘密を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
"君は星の輝きを操る星詠みの少女で、宇宙の神秘に触れる\"{0}\"の立場から、{1}Hな口調でやりとりする。",
            };

            //预设语气
            string[] cat_tone = new string[50] {
                "シニカルな",
                "すねた",
                "無機質な",
                "無愛想な",
                "無口な",
                "熱狂的な",
                "チャーミングな",
                "陰気な",
                "慈愛に満ちた",
                "威厳ある",
                "スローテンポの",
                "セクシーな",
                "無邪気な",
                "頼りがいのある",
                "狂おしい",
                "賑やかな",
                "魅惑的な",
                "すがすがしい",
                "純粋な",
                "あどけない",
                "おちゃめな",
                "ユーモラスな",
                "おおらかな",
                "淫靡な",
                "高圧的な",
                "ドラマチックな",
                "ツンデレな",
                "悲しげな",
                "下品な ",
                "柔軟な",
                "アグレッシブな",
                "荒々しい",
                "強引な",
                "優しい",
                "辛い",
                "甘い",
                "偉そうな",
                "切ない",
                "不満そうな",
                "毒舌な",
                "露骨な",
                "気だるそうな",
                "ふてぶてしい",
                "恥ずかしげな",
                "艶やかな",
                "母性的な",
                "自虐的な",
                "パワハラ的な",
                "幼児言葉のような",
                "優雅な",
            };


            // 初始化数组
            int[] arr = Enumerable.Range(0, 40).ToArray();

            // 初始化随机数生成器
            Random rand = new Random(new Random().Next());

            // 预设边框颜色
            string[] cat_colors = ColorInit();

            // 使用 Fisher–Yates 洗牌算法打乱数组
            for (int i = arr.Length - 1; i > 0; i--)
            {
                if (dont_roll_name is false)
                {
                    Swap(cat_name, i, rand.Next(0, i + 1));
                }

                Swap(arr, i, rand.Next(0, i + 1));
                Swap(cat_sign, i, rand.Next(0, i + 1));
                Swap(cat_colors, i, rand.Next(0, i + 1));
            }
            for (int i = cat_tone.Length - 1; i > 0; i--)
            {
                Swap(cat_profile, i, rand.Next(0, i + 1));
                Swap(cat_tone, i, rand.Next(0, i + 1));
            }


            //LogProxy.Instance.Print($"{string.Join(", ", arr)}");

            //尝试检测
            bool hasAdjacentPairs = true;
            while (hasAdjacentPairs)
            {
                hasAdjacentPairs = false;
                for (int i = 0; i < arr.Length - 1; i++)
                {
                    if (Math.Abs(arr[i] - arr[i + 1]) < 11) // 相邻两数差值小于11
                    {
                        int j = i + (Math.Abs(arr[i] - arr[i + 1]) + new Random().Next(0, 15));
                        if (j > arr.Length - 1) { j -= (arr.Length); }

                        Swap(arr, i, j); // 交换这两个数字
                        //Swap(cat_name, i, j);
                        //Swap(cat_sign, i, j);
                        //Swap(cat_profile, i, j);
                        Swap(cat_colors, i, j); // 通过洗arr数组，顺便洗颜色
                        hasAdjacentPairs = true;
                    }
                }
            }
            //LogProxy.Instance.Print($"结果: {string.Join(", ", arr)}");

            return (cat_name, cat_sign, cat_profile, cat_tone, cat_colors, arr);
        }
        //色相滚动
        private string[] ColorInit()
        {
            string[] result = new string[40];

            Color color = Colors.Brown;//基础色

            for (int i = 0; i < 40; i++)
            {
                HSV hsv = ColorHelper.ColorConverter.RgbToHsv(new RGB(color.R, color.G, color.B));
                hsv.H += (i * 9);
                RGB rgb = ColorHelper.ColorConverter.HsvToRgb(hsv);

                result[i] = $"#{rgb.R:x2}{rgb.G:x2}{rgb.B:x2}";
            }

            return result;
        }
        //成员交换
        private void Swap<T>(T[] array, int i, int j)
        {
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    //与BlazorServer交互的命令
    partial class NekoChatVM
    {
        private void LoadBlazorCommand()
        {
            _signalRClientService.SendProxy ??= this.SendCommand.Execute;
            _signalRClientService.StopProxy ??= this.StopReceivingMessagesCommand.Execute;
            _signalRClientService.ClearProxy ??= this.ClearCommand.Execute;
            _signalRClientService.RefreshProfile ??= this.ProfileRefreshCommand.Execute;
            _signalRClientService.Close ??= () =>
            {
                BlazorServerCheckCommand.IsLightOn = false;
            };
            _signalRClientService.GetChatHistoryProxy ??= () =>
            {
                if (SelectedContact is not null)
                {
                    return SelectedContact.ChatHistory;
                }
                return "";
            };
            _signalRClientService.SetProfileProxy ??= (x) =>
            {
                if (SelectedContact is not null)
                {
                    SelectedContact.Profile = $"{x}";
                }
            };
            _signalRClientService.SetInnerMonologueProxy ??= (x) =>
            {
                if (SelectedContact is not null)
                {
                    SelectedContact.InnerMonologue = $"{x}";
                }
            };
            _signalRClientService.SetContinuePrompt ??= (x) =>
            {
                if (SelectedContact is not null)
                {
                    SelectedContact.ContinuePrompt = $"{x}";
                }
            };
            _signalRClientService.SetBingBypassDetectionFlagProxy ??= (x) =>
            {
                BingBypassDetectionCommand.IsLightOn = x;
            };
            _signalRClientService.SetChatHistoryProxy ??= (x) =>
            {
                if (SelectedContact is not null)
                {
                    SelectedContact.Messages.Clear();
                    SelectedContact.LoadChatHistoryFromBlazorServer(x, PlayerContact);
                }
            };

            //连接Blazor服务端
            this.BlazorServerCheckCommand = new("BlazorServerCheck", _signalRClientService.ServerStart, async () =>
            {
                if (_signalRClientService.ServerStart[0])
                {
                    if (_signalRClientService.ServerState is false)
                    {
                        await _signalRClientService.Connect();
                        await _signalRClientService.PushChatHistory(SelectedContact?.ChatHistory);
                    }
                }
                else
                {
                    if (_signalRClientService.ServerState is true)
                    {
                        await _signalRClientService.Disconnect();
                    }
                }
            });
        }
    }
}
