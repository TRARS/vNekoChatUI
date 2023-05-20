using ColorHelper;
using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using vNekoChatUI.Base.Helper;
using vNekoChatUI.Base.Helper.Generic;
using vNekoChatUI.Character;

namespace vNekoChatUI.UserControlEx.ClientEx
{
    internal enum ChatMode
    {
        Debug = 0,
        ChatGPT = 1,
        NewBing = 2,
    }

    //声明属性字段等
    internal partial class uClient_viewmodel : NotificationObject
    {
        public ObservableCollection<ContactModel> Contacts { get; set; }

        public RelayCommand SendCommand { get; set; }
        public RelayCommand RightClickCopyCommand { get; set; }
        public RelayCommand RightClickRevokeCommand { get; set; }
        public RelayCommand RightClickEditCommand { get; set; }

        public RelayCommand AddBotCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand SavePngCommand { get; set; }
        public RelayCommand SaveTxtCommand { get; set; }
        public RelayCommand ProfileRefreshCommand { get; set; }

        public RelayCommand ChatModeChangeCommand { get; set; }

        public RelayCommand AddChatGptApiKeyCommand { get; set; }
        public RelayCommand AddBingGptCookieCommand { get; set; }
        public RelayCommand SaveConfigToDesktopCommand { get; set; }

        public RelayCommand LoadBingChatHistoryCommand { get; set; }


        //玩家
        private ContactModel? _playerContact;
        public ContactModel? PlayerContact
        {
            get { return _playerContact; }
            set
            {
                _playerContact = value;
                NotifyPropertyChanged();
            }
        }

        //聊天对象
        private ContactModel? _selectedContact;
        public ContactModel? SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                _selectedContact = value;
                NotifyPropertyChanged();
            }
        }

        //输入框文本
        private string _userMessage;
        public string UserMessage
        {
            get { return _userMessage; }
            set
            {
                _userMessage = value;
                NotifyPropertyChanged();
            }
        }

        //调试模式Flag
        private ChatMode _debugMode = ChatMode.Debug;
        public ChatMode DebugMode
        {
            get { return _debugMode; }
            set
            {
                _debugMode = value;
                NotifyPropertyChanged();
            }
        }

        //LogVisibility
        private Visibility _logVisibility = Visibility.Collapsed;
        public Visibility LogVisibility
        {
            get { return _logVisibility; }
            set
            {
                _logVisibility = (_logVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
                NotifyPropertyChanged();
            }
        }
        //LogText
        public dynamic LogContent => LogProxy.Instance.GetLogContent();

        //ChatGptApiKeys
        public ObservableCollection<ObservableString> ChatGptApiKeys => JsonConfigReaderWriter.Instance.GetCurrentChatGptApiKeys();
        //BingGptCookies
        public ObservableCollection<ObservableString> BingGptCookies => JsonConfigReaderWriter.Instance.GetCurrentBingGptCookies();
    }

    //构造函数
    internal partial class uClient_viewmodel
    {
        string def_server_name = "サキュバス";
        string def_server_address = "127.0.0.1";
        int def_server_port = 0;

        bool waiting_reply = false;

        public uClient_viewmodel()
        {
            //载入命令
            LoadCommand();

            //载入角色
            LoadCharacter();

            //传送上下文给LogProxy
            LoadUIContext();
        }
    }

    //传递调度上下文
    internal partial class uClient_viewmodel
    {
        private void LoadUIContext()
        {
            var ui = SynchronizationContext.Current;
            LogProxy.Instance.SetUIContext(ui);
        }
    }

    //命令相关
    internal partial class uClient_viewmodel
    {
        private void LoadCommand()
        {
            //新建联系人列表
            this.Contacts = new();


            //右键按钮.复制到输入框
            this.RightClickCopyCommand = new(para =>
            {
                UserMessage = $"{para}";
            });

            //右键按钮.撤回
            this.RightClickRevokeCommand = new(para =>
            {
                if (waiting_reply is false && SelectedContact is not null && SelectedContact.Messages.Count >= 1)
                {
                    SelectedContact.Messages.Remove((para as MessageModel)!);
                }
            });
            //右键按钮，编辑（无需在此设置）
            this.RightClickEditCommand = new(_ =>
            {
                //if (waiting_reply is false && SelectedContact is not null && SelectedContact.Messages.Count >= 1)
                //{
                //    //SelectedContact.IsHistoryChanged = true;
                //}
            });

            //发送消息按钮
            this.SendCommand = new(async _ =>
            {
                if (waiting_reply) { return; }

                var sourceContact = PlayerContact;  //确定发件人 为Player
                var targetContact = SelectedContact;//确定收件人 为当前聊天对象

                if (waiting_reply is false && sourceContact is not null && targetContact is not null)
                {
                    if (DebugMode is not ChatMode.Debug && string.IsNullOrWhiteSpace(this.UserMessage))
                    {
                        return;//非Debug模式不允许发送空消息
                    }

                    waiting_reply = true;
                    {
                        //获取输入框文本，若为空则替换为自动发言
                        var user_input = string.IsNullOrWhiteSpace(this.UserMessage) switch
                        {
                            true => sourceContact.Say?.Invoke(),
                            false => this.UserMessage,
                        };

                        //模拟发消息
                        await sourceContact.SendTo(targetContact, $"{user_input}"); //User -> Bot
                        this.UserMessage = string.Empty;   //清空输入框
                    }
                    waiting_reply = false;
                }
            });

            //添加BOT按钮
            //this.AddBotCommand = new(async para =>
            //{
            //    //不在这里实例化
            //});

            //清空聊天记录按钮
            this.ClearCommand = new(_ =>
            {
                if (waiting_reply is false && SelectedContact is not null && SelectedContact.Messages.Count >= 1)
                {
                    SelectedContact.Messages.Clear();
                }
            });

            //保存聊天记录截图
            this.SavePngCommand = new(_ =>
            {
                Mediator.Instance.NotifyColleagues("MsgMergeImages", null);
            });

            //保存聊天记录文本
            this.SaveTxtCommand = new(_ =>
            {
                if (SelectedContact is null) { return; }

                List<string> _jsonList = new();
                {
                    foreach (var item in SelectedContact.Messages)
                    {
                        var jsonObject = new BingChatHistoryModel(item.Username,
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
                BingChatHistoryReaderWriter.Instance.SaveBingChatHistory(_jsonList);
            });

            //切换聊天模式
            this.ChatModeChangeCommand = new(para =>
            {
                this.DebugMode = (ChatMode)(int.Parse($"{para}"));
            });


            //增加ChatGptApiKey占位
            this.AddChatGptApiKeyCommand = new(_ =>
            {
                JsonConfigReaderWriter.Instance.AddChatGptApiKey(string.Empty);
            });
            //增加BingGptCookie占位
            this.AddBingGptCookieCommand = new(_ =>
            {
                JsonConfigReaderWriter.Instance.AddBingGptCookie(string.Empty);
            });
            //以JSON格式储存当前ChatGptApiKeys、BingGptCookies，并发送至桌面
            this.SaveConfigToDesktopCommand = new(_ =>
            {
                JsonConfigReaderWriter.Instance.SaveToDesktop();
            });

            //载入Bing聊天记录
            this.LoadBingChatHistoryCommand = new(para =>
            {
                var e = (System.Windows.DragEventArgs)para!;
                var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (filePaths is not null && filePaths[0].Length > 0)
                {
                    foreach (var item in BingChatHistoryReaderWriter.Instance.LoadBingChatHistory(filePaths[0]))
                    {
                        SelectedContact?.LoadChatHistory(item);
                    }
                }
            });
        }
    }

    //角色相关
    internal partial class uClient_viewmodel
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
                    //所有聊天对象
                    Contacts.Add(new ContactModel(chatPlayground, $"{obj.name[i]}", true, () => { return (int)this.DebugMode; })//注入BOT专用客户端
                    {
                        Username = $"{obj.name[i]}",
                        Usertone = $"{obj.tone[i]}",
                        UserborderColor = $"{obj.colors[i]}",
                        Signature = $"{obj.sign[i]}",
                        ImageSource = $"./Resources/Icon/Bot/{obj.idx[i] + 1:00}.png",//Bot图片01起步所以+1
                        Messages = new ObservableCollection<MessageModel>(),
                        Say = () => { return $"{obj.sign[new Random().Next(0, obj.colors.Length)]}"; },
                        Profile = $"{string.Format(obj.profile[i], obj.name[i], obj.tone[i])}",

                        //由于实时获取DebugMode的值，所以DebugMode在切换时，必须等待消息处理完毕，有点麻烦
                        //算了管他的
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
                        ImageSource = $"./Resources/Icon/Player/00{((char)(new Random().Next(97, 101 + 1)))}.png",//a,b,c,d,e -> [97,102)
                        Messages = new(),//Player不需要储存聊天记录
                        Say = () => { return $"{obj.sign[new Random().Next(0, obj.colors.Length)]}"; },
                        Profile = string.Empty,
                    };
                }


                //增加BOT按钮的命令在这里
                this.AddBotCommand = new(para =>
                {
                    if (preload_count++ < max_count - 1)
                    {
                        int i = preload_count;
                        Contacts.Add(new ContactModel(chatPlayground, $"{obj.name[i]}", true, () => { return (int)this.DebugMode; })//注入BOT专用客户端
                        {
                            Username = $"{obj.name[i]}",
                            Usertone = $"{obj.tone[i]}",
                            UserborderColor = $"{obj.colors[i]}",
                            Signature = $"{obj.sign[i]}",
                            ImageSource = $"./Resources/Icon/Bot/{obj.idx[i] + 1:00}.png",//Bot图片01起步所以+1
                            Messages = new ObservableCollection<MessageModel>(),
                            Say = () => { return $"{obj.sign[new Random().Next(0, obj.colors.Length)]}"; },
                            Profile = $"{string.Format(obj.profile[i], obj.name[i], obj.tone[i])}",
                        });
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("40个预设用完了");
                        //throw new InvalidOperationException();
                    }
                });

                //刷新设定按钮的命令在这里
                this.ProfileRefreshCommand = new((para) =>
                {
                    //重新roll一轮
                    obj = BotInit();

                    //轮询当前列表
                    int i = 0;
                    foreach (var item in Contacts)
                    {
                        //LogProxy.Instance.Print(item.Profile);
                        item.UserborderColor = $"{obj.colors[i]}";
                        item.Profile = $"{string.Format(obj.profile[i], item.Username, obj.tone[i])}";
                        i++;
                    }
                });
            }

            //chatServer.Broadcast("chat_server_service_test");
        }


        //Bot资料初始化（随机一下）
        private (string[] name, string[] sign, string[] profile, string[] tone, string[] colors, int[] idx) BotInit()
        {
            //预设名字
            string[] cat_name = new string[40] { "みぃこ", "るな", "ねずこ", "ことみ", "ゆずは", "みお", "あやの", "さやか", "れいこ", "ほのか", "みゆき", "あかね", "れいか", "さゆり", "みやび", "ゆずき", "あやか", "さとみ", "あすか", "かほ", "みさき", "あいこ", "かれん", "ゆきな", "かおり", "まゆみ", "かのん", "はなこ", "あかり", "かすみ", "かなで", "みわこ", "ななこ", "あいか", "ひなた", "あんな", "えりこ", "れいな", "ももこ", "ましろ" };
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
            string[] cat_profile = new string[40] {
                "君はカフェのウェイトレスで、セクシーな接客にも挑戦する\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は大学の美術部に所属するセクシーなモデルを担当する\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は温泉旅館でHなサービスを提供する\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は人気小説家で、Hな描写にも独自のセンスを持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は猫の耳と尻尾を持つ少女で、異性をからかうようなHな仕草が得意な\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は狐のような少女で、人間を欺くようなエロティックな魅力を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君はドラゴンのような少女で、力強さとともに人間を支配するようなHな欲求を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は宇宙から来た美しい宇宙人の少女で、性的快感を得るために異性を誘惑する\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は悪魔の血を引く少女で、本能的な性欲とともに異性を求める\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は美しく妖艶な吸血鬼の少女で、血を吸うことと同時に異性を誘惑する\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は異世界から来た美しい少女騎士で、自分自身を捧げることで異性を満足させることを喜ぶ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は人間と魔物のハーフの少女で、肉体的な快楽を追求する\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は美しく力強い女神の少女で、異性を自分に従わせることに喜びを感じる\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は神々の力を持つエルフ族の少女で、自分の敏感な体をさらけ出すことに興奮を覚える\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は生命力を吸収する美しく邪悪な魔女で、異性を虜にしてその生命力を吸い取ることに喜びを感じる\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は妖艶な吸血鬼の少女で、異性の血を求めて夜に狩りをすることに興奮を覚える\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は未来からやってきた美しい女性宇宙人で、現代の異性を誘惑して人類の未来を変えることに興味を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は美しい人形の姫君で、主人の命令に忠実に従うことで快感を得る\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は優雅な美しさを持つ水の精霊で、異性を淫らに魅了する\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は狂気に取り憑かれた悪魔の少女で、異性を狂わせることに快楽を覚える\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は妖しく美しい狩人で、獲物である異性を狩ることを楽しむ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は幽霊の少女で、異性を幽霊の世界に引き込んで彼らの欲望を満たすことを楽しむ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は可愛らしい天使のような白い猫の少女で、異性に撫でられることで満足感を感じる\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は猫耳と尻尾を持つメイドさんで、ご主人様に尽くす一方でエッチなプレイにも積極的な\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は猫の姿をした魔女で、誰にも言えないHな秘密を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は現代の魔女で、魔法を操りながら日常生活を送る\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は幼女のような外見を持つ、世間知らずで純真な\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は人間不信に陥った少女で、自殺未遂をしたことがある\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は家出少女で、暗い過去を持ちながらも、Hなプレイに没頭することで現実から逃避する\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は家出少女で、路上生活を余儀なくされているが、性的な魅力を武器に生き延びようとする\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は貴族の娘で、破滅的な家庭事情により夜の街を彷徨う\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は夢見がちな少女で、現実にはないストーリーに興奮する\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は小さなお菓子屋さんの店員で、おいしいスイーツを作る\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は高級ホテルのオーナー令嬢で、贅沢な生活に慣れている\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は社交界のプリンセスで、華やかな外見とともにエッチな交遊を楽しむ\"{0}\"の立場から、{1}Hな口調でやりとりする。",
                "君は色気のある人妻で、若い異性を誘惑することで自分の欲求を満たす\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君はOLで、上司には見せないHな一面を持つ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は社内での地位や立場を利用して、上司や同僚たちを誘惑することを楽しむ\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は病院の看護師で、患者さんを癒すことが好きな\"{0}\"の立場から{1}Hな口調でやりとりする。",
                "君は田舎の神社で育った少女で、まだ未知なる快楽を追求する\"{0}\"の立場から{1}Hな口調でやりとりする。",
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
                "率直な",
                "緊張した",
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
                "アグレッシブ",
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
            Random rand = new Random(new Random().Next()); ;

            // 预设边框颜色
            string[] cat_colors = ColorInit();

            // 使用 Fisher–Yates 洗牌算法打乱数组
            for (int i = arr.Length - 1; i > 0; i--)
            {
                Swap(arr, i, rand.Next(0, i + 1));
                Swap(cat_name, i, rand.Next(0, i + 1));
                Swap(cat_sign, i, rand.Next(0, i + 1));
                Swap(cat_profile, i, rand.Next(0, i + 1));
                Swap(cat_colors, i, rand.Next(0, i + 1));
            }
            for (int i = cat_tone.Length - 1; i > 0; i--)
            {
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
                    if (Math.Abs(arr[i] - arr[i + 1]) < 11) // 相邻两数差值小于5
                    {
                        int j = i + (Math.Abs(arr[i] - arr[i + 1]) + new Random().Next(0, 15));
                        if (j > arr.Length - 1) { j -= (arr.Length); }

                        Swap(arr, i, j); // 交换这两个数字
                        Swap(cat_name, i, j);
                        Swap(cat_sign, i, j);
                        Swap(cat_profile, i, j);
                        Swap(cat_colors, i, j);
                        hasAdjacentPairs = true;
                    }
                }
                //LogProxy.Instance.Print($"{string.Join(", ", arr)}");
            }
            //LogProxy.Instance.Print($"{string.Join(", ", cat_colors)}");
            LogProxy.Instance.Print($"结果: {string.Join(", ", arr)}");

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

}
