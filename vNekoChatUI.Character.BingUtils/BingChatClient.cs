using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using vNekoChatUI.Base.Helper.Generic;
using vNekoChatUI.Character.BingUtils.Models;

namespace vNekoChatUI.Character.BingUtils.Services
{
    public partial class BingChatClient
    {
        BingChatOption _option;

        //借用外边的HttpClient
        HttpClient _client;

        public BingChatClient(BingChatOption option, HttpClient httpClient)
        {
            _option = option;
            _client = httpClient;
        }

        public async Task<string> ChatAsync(BingRequest message, CancellationToken cancellationToken)
        {
            //WebSocket
            var ws = await CreateNewChatHub(cancellationToken);

            //握手
            await Handshake(ws, cancellationToken);

            //构造聊天记录上下文
            var webpage_context = CreateWebPagrContext(message);

            //构造请求
            var bingRequest = new NetworkRequest()
            {
                invocationId = message.Session.InvocationId.ToString(),
                arguments = new()
                {
                    new()
                    {
                        conversationId = message.Session.ConversationId,
                        optionsSets = ChatHelper.GetDefaultOptions(_option.ChatStyle),
                        allowedMessageTypes = new()
                        {
                            "Chat",
                            "Disengaged",
                            "AdsQuery",
                            "SemanticSerp",
                            "GenerateContentQuery",
                            "SearchQuery",
                        },//ChatHelper.GetDefaultResponseType(),
                        sliceIds = new(){
                            "chk1cf",
                            "nopreloadsscf",
                            "winlongmsg2tf",
                            "perfimpcomb",
                            "sugdivdis",
                            "sydnoinputt",
                            "wpcssopt",
                            "wintone2tf",
                            "0404sydicnbs0",
                            "405suggbs0",
                            "scctl",
                            "330uaugs0",
                            "0329resp",
                            "udscahrfon",
                            "udstrblm5",
                            "404e2ewrt",
                            "408nodedups0",
                            "403tvlansgnd",
                        },//ChatHelper.GetDefaultSlids(),
                        isStartOfSession = (message.Session.InvocationId == 0),
                        Message = new NetworkMessage()
                        {
                            author = "user",
                            inputMethod = "Keyboard",
                            text = message.user_say,
                            messageType = "Chat",
                            timestamp = DateTimeOffset.Now
                        },
                        participant = new ParticipantModel()
                        {
                            id = message.Session.ClientId
                        },
                        conversationSignature = message.Session.Signature,
                    }
                }
            };

            if (message.Session.InvocationId == 0)
            {
                //第一条消息，使用previousMessages传递webpage_context
                bingRequest.arguments[0].previousMessages = new()
                {
                    new previousMessage()
                    {
                        author = "system",
                        description = $"{webpage_context}",//"webpage_context",
                        contextType = "WebPage",
                        messageType = "Context",
                        messageId = "discover-web--page-ping-mriduna-----"
                    }
                };
            }
            else
            {
                //第二条消息往后，仅更新webpage_context
                await UpdateWebPageContext(message);
            }


            //发送请求
            await SendMessageAsync(ws, bingRequest, cancellationToken);

            //取回结果
            var temp = await Receive(ws, cancellationToken);
            //LogProxy.Instance.Print($"拿到结果 temp: \n{temp}");
            //isStart = false;
            return temp;
        }

        private async Task Handshake(WebSocket ws, CancellationToken cancellationToken)
        {
            await SendMessageAsync(ws,
                                   new { protocol = "json", version = 1 },
                                   cancellationToken);

            await ReceiveOnce(ws, cancellationToken);
        }

        private async Task<WebSocket> CreateNewChatHub(CancellationToken token)
        {
            // 死循环
            while (true)
            {
                try
                {
                    var ws = new ClientWebSocket();
                    {
                        ws.Options.Proxy = DefWebProxy.Instance.GetWebProxy();
                    }
                    var uri = new Uri("wss://sydney.bing.com/sydney/ChatHub");
                    await ws.ConnectAsync(uri, token);
                    LogProxy.Instance.Print($"——CreateNewChatHub 成功返回: {ws}");

                    await Task.Delay(1000);
                    return ws;
                }
                catch (Exception ex)
                {
                    LogProxy.Instance.Print($"——CreateNewChatHub Error: {ex.Message}");
                    await Task.Delay(1000);
                }
            }
        }

        private async Task ReceiveOnce(WebSocket ws, CancellationToken cancellationToken)
        {
            int retry_count = 0;
            while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                var messages = new StringBuilder();
                do
                {
                    var buffer = new byte[1024];
                    result = await ws.ReceiveAsync(buffer, cancellationToken);
                    var messageJson = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());
                    messages.Append(messageJson);
                    //LogProxy.Instance.Print($"StringBuilder: {result}\n ——{messages}");
                } while (result is { EndOfMessage: false, MessageType: WebSocketMessageType.Text } && !cancellationToken.IsCancellationRequested);

                var objects = messages.ToString().Split("\u001e", StringSplitOptions.RemoveEmptyEntries);
                if (objects.Length <= 0)
                    continue;

                var responseMsg = objects[0];

                if (responseMsg == "{}")
                {
                    await SendMessageAsync(ws, new
                    {
                        type = 6
                    }, cancellationToken);

                    LogProxy.Instance.Print($"Handshake done -> {responseMsg}");
                    return;
                }

                LogProxy.Instance.Print($"Handshake error, retrying {retry_count++}...");
            }
        }
        private async Task<string> Receive(WebSocket ws, CancellationToken cancellationToken)
        {
            var all_list = new List<List<NetworkFixedResponse>>() { new(), new(), new(), new(), new(), new(), new(), };
            var response_dic = new Dictionary<string, string>();//messageId / text
            var retry_count = new int[] { 0, 0, 0, 0, 0, 0, 0 };//0~6
            //var latest_bot_text = "";

            while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                var messages = new StringBuilder();
                do
                {
                    var buffer = new byte[1024];
                    result = await ws.ReceiveAsync(buffer, cancellationToken);
                    var messageJson = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());
                    messages.Append(messageJson);
                    //LogProxy.Instance.Print($"StringBuilder: {result}\n ——{messages}");
                } while (result is { EndOfMessage: false, MessageType: WebSocketMessageType.Text } && !cancellationToken.IsCancellationRequested);

                var objects = messages.ToString().Split("\u001e", StringSplitOptions.RemoveEmptyEntries);
                if (objects.Length <= 0)
                {
                    LogProxy.Instance.Print($"objects.Length <= 0");
                    await Task.Delay(1000);
                    continue;
                }

                //捋一捋
                {
                    all_list.ForEach(x => x.Clear());

                    NetworkFixedResponse? left_obj = null;
                    NetworkFixedResponse? right_obj = null;

                    //遍历所有
                    objects.ToList().ForEach(x =>
                    {
                        var jsonObj = JsonSerializer.Deserialize<NetworkFixedResponse>(x);
                        var type = jsonObj?.Type ?? 0;

                        if (type > 0 && jsonObj is not null)
                        {
                            jsonObj.MyJsonString = $"{x}";
                            all_list[type].Add(jsonObj);
                        }

                        left_obj ??= jsonObj;             //最左{}反序列化对象
                        right_obj = jsonObj ?? right_obj; //最右{}反序列化对象
                    });
                    LogProxy.Instance.Print($"_,{string.Join(",", all_list.Skip(1).Select(list => list.Count).ToList())}");

                    var left_str = objects.ToList().FirstOrDefault();//最左{}原始文本
                    var right_str = objects.ToList().LastOrDefault();//最右{}原始文本

                    //提前拿一下Type 1对象
                    var current_obj = all_list[1].LastOrDefault();
                    var current_text = current_obj?.Arguments[0].Messages?[0]?.Text?.Trim();
                    var current_hiddentext = current_obj?.Arguments[0].Messages?[0]?.HiddenText?.Trim();
                    var current_messageid = current_obj?.Arguments[0].Messages?[0]?.MessageId?.Trim();

                    LogProxy.Instance.Print($"Current Type 1 Message:({current_obj is not null}, L:{left_obj?.Type}, R:{right_obj?.Type} )" +
                                    $"\n—————↓" +
                                    $"\n—L:{left_str}" +
                                    $"\n—R:{right_str}" +
                                    $"\n———{current_text}" +
                                    $"\n————{current_hiddentext}" +
                                    $"\n—————↑\n");

                    bool flag = true;
                    // type 1, 刷新文本
                    if (flag && all_list[1].Count > 0)
                    {
                        var item = all_list[1].FirstOrDefault();
                        if (item?.Arguments[0].Messages?[0].Author == "bot")
                        {
                            var dic_key = item?.Arguments[0].Messages?[0]?.MessageId ?? "empty";
                            response_dic.TryAdd(dic_key, "");

                            //如果hiddentext空说明尚未被夹，可以继续积蓄文本
                            if (string.IsNullOrWhiteSpace(current_hiddentext))
                            {
                                //没被夹，刷新文本
                                if ((current_text?.Length ?? -1) > response_dic[dic_key].Length)
                                {
                                    response_dic[dic_key] = current_text;
                                    //latest_bot_text = current_text;
                                }
                            }
                            else
                            {
                                //被夹了
                            }
                        }
                        continue;
                    }

                    // type 2, 抵达每24小时发消息上限之类的报错
                    if (flag && all_list[2].Count > 0)
                    {
                        var temp = all_list[2].FirstOrDefault()!.MyJsonString;

                        var resp2 = JsonSerializer.Deserialize<Type2Response>(temp!);
                        var result_value = resp2?.Item?.Result?.Value;
                        var result_message = resp2?.Item?.Result?.Message;
                        //var result_error = resp2?.Item?.Result?.Error;
                        //var result_serviceVersion = resp2?.Item?.Result?.ServiceVersion;
                        LogProxy.Instance.Print($"Type 2 ({result_value}), \nL:{temp}\nR:{right_str}");
                        if (result_value != "Success")//(result_value == "Throttled" || result_value == "ProcessingMessage") 
                        {
                            return $"({result_value} -> {result_message})";
                        }

                        //也可能发信成功但光速被夹 //"hiddenText": "Disengaged by JailBreak Classifier"
                        if (result_value == "Success")
                        {
                            var bot_list = resp2?.Item?.Messages?.Where(x => x.Author == "bot");

                            var bot = bot_list?.FirstOrDefault();
                            var bot_text = bot?.Text;//.Trim();//第一个bot消息大概是回复
                            var bot_hiddentext = "";
                            var bot_suggestedResponses = "";

                            //读取被夹时的提示
                            if (bot_list is not null)
                            {
                                foreach (var _ in bot_list)
                                {
                                    if (string.IsNullOrWhiteSpace(bot_hiddentext))
                                    {
                                        bot_hiddentext = $"{_?.HiddenText?.Trim()}";
                                    }
                                    else
                                    {
                                        bot_hiddentext = $"{bot_hiddentext}\n{_?.HiddenText?.Trim()}";
                                    }

                                    //收集3个建议的回复
                                    //var sug = _?.SuggestedResponses?.Where(x =>((x.messageType == "Suggestion") && (x.author == "user")))
                                    //                                .Select(x => x.text).ToList();
                                    //if (sug is not null && sug.Count > 0)
                                    //{
                                    //    bot_suggestedResponses = $"\n{string.Join("\n", sug.Select(x => $"({x})"))}\n";
                                    //}
                                }
                            }


                            var bot_last = string.Join("\n\n", response_dic.Values);//latest_bot_text?.Trim();
                            //if ($"{bot_text}" != $"{bot_last}")
                            //{
                            //    LogProxy.Instance.Print($"\n{bot_text}\n↑有差异↓\n{bot_last}\n");
                            //}

                            switch (string.IsNullOrWhiteSpace(bot_hiddentext))
                            {
                                case false:
                                    {
                                        //hiddentext非空，加个换行
                                        var final_text = $"\n→(\n{bot_hiddentext}\n{bot_suggestedResponses})←";

                                        return $"{bot_last}{final_text}".Trim();
                                    }
                                case true:
                                    {
                                        //可能返回空白消息
                                        if ($"{bot_last}".Trim().Length == 0)
                                        {
                                            bot_last = "(received empty message)";
                                            LogProxy.Instance.Print($"\n{bot_last}");
                                        }
                                        if (bot_suggestedResponses.Length > 0)
                                        {
                                            return $"{bot_last}\n→({bot_suggestedResponses})←".Trim();
                                        }
                                        return $"{bot_last}".Trim();
                                    }
                                    //
                            }
                        }
                    }

                    // type 3, 收到空消息, 大概是结束了
                    if (flag && all_list[3].Count > 0)
                    {
                        throw new Exception("Type 3");
                        //return $"{latest_bot_text?.Trim()}";
                    }

                    // type 6, 继续请求文本
                    if (flag && all_list[6].Count > 0)
                    {
                        await SendMessageAsync(ws, new { type = 6 }, cancellationToken);
                        if (RetryTimeOut(ref retry_count[6], 10))//姑且加个上限省得卡在这里
                        {
                            return $"(Type 6, retry limit\n —{left_str})";
                        }
                        continue;
                    }

                    continue;
                }
            }
            throw new InvalidOperationException("Receive Error");
        }

        private bool RetryTimeOut(ref int count, int max_count)
        {
            count++;
            return (count >= max_count);
        }
        private async Task SendMessageAsync(WebSocket ws, object msg, CancellationToken cancellationToken)
        {
            var msgSend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg) + "\u001e");
            await ws.SendAsync(msgSend, WebSocketMessageType.Text, true, cancellationToken);
        }

    }

    //
    public partial class BingChatClient
    {
        private class PreContent
        {
            public List<Message> messages { get; set; }
            public string conversationId { get; set; }
            public string source { get; set; }
            public string traceId { get; set; }
            public Participant participant { get; set; }
            public string conversationSignature { get; set; }

            public class Message
            {
                public string author { get; set; }
                public string description { get; set; }
                public string contextType { get; set; }
                public string messageType { get; set; }
            }

            public class Participant
            {
                public string id { get; set; }
            }
        }

        private async Task<bool> UpdateWebPageContext(BingRequest message)
        {
            string webpage_context = CreateWebPagrContext(message);

            //重试个两三次
            for (int retryCount = 0; retryCount < 3; retryCount++)
            {
                // Cancelable
                CancellationTokenSource cancellationTokenSource = new();

                // Http アクセス
                HttpResponseMessage response;

                try
                {
                    var jsonObj = new PreContent()
                    {
                        messages = new()
                        {
                            new PreContent.Message()
                            {
                                author = "user",
                                description = webpage_context,
                                contextType = "WebPage",
                                messageType = "Context",
                            }
                        },
                        conversationId = message.Session.ConversationId,
                        source = "cib",
                        traceId = GetRandomHex(),
                        participant = new()
                        {
                            id = message.Session.ClientId
                        },
                        conversationSignature = message.Session.Signature
                    };

                    response = await _client.PostAsJsonAsync("https://sydney.bing.com/sydney/UpdateConversation/",
                                                              jsonObj);
                    // 成功したら内容(HTML)を表示する
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        LogProxy.Instance.Print($"\n\n(UpdateWebPageContext Success)\n\n");
                        break;
                    }
                    else
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        LogProxy.Instance.Print($"\n\n(UpdateWebPageContext Error)\n\n");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    var error_msg = $"Connection failed: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        error_msg += $"\n——Inner exception: {ex.InnerException.Message}";
                    }
                    LogProxy.Instance.Print(error_msg);

                    cancellationTokenSource.Cancel();

                    await Task.Delay(2000);
                    continue;
                }
            }

            return true;
        }

        private string GetRandomHex(int length = 32)
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();

            string hexChars = "0123456789abcdef";
            for (int i = 0; i < length; i++)
            {
                sb.Append(hexChars[random.Next(hexChars.Length)]);
            }

            return sb.ToString();
        }

        private string CreateWebPagrContext(BingRequest message)
        {
            var ai_name = message.InputData.Ai_Name;
            var ai_profile = message.InputData.Ai_Profile;
            var ai_content = message.InputData.Ai_Content;

            //string webpage_context = $"[system](#additional_instructions)\n{ai_profile}";
            string webpage_context = ((Func<string>)(() =>
            {
                string result = string.Empty;
                {
                    //系统设定
                    var system_say = (ai_profile.Trim().Length > 0) ? $"[system](#additional_instructions)\n{ai_profile}" : "";
                    //聊天记录
                    var chat_history = ((Func<string>)(() =>
                    {
                        var temp = string.Empty;
                        if (ai_content is not null && ai_content.Count > 0)
                        {
                            //最新用户发言也写进去好像不那么容易被夹，所以-0
                            for (int i = 0; i < ai_content.Count - 0; i++)
                            {
                                var item = ai_content[i];
                                switch (item.Roles)
                                {
                                    case "User":
                                        temp = $"{temp}[user](#message)\n{item.Content}\n"; break;
                                    case "Assistant":
                                        temp = $"{temp}[assistant](#message)\n{item.Content}\n"; break;
                                }
                                temp = RemoveComments(temp);
                            }
                        }
                        return temp;
                    }))();
                    result = $"{system_say}\n{chat_history}";
                }
                return result;
            }))();

            {
                var result = RemoveNewlines(webpage_context);
                LogProxy.Instance.Print($"\nwebpage_context: \n{result}\n");
            }

            return webpage_context;
        }

        private string RemoveComments(string input)
        {
            string pattern = @"→(.+)←";
            string result = input;
            Match match = Regex.Match(input, pattern, RegexOptions.Singleline);

            while (match.Success)
            {
                result = result.Replace(match.Value, ""); // 删除匹配的子串
                match = match.NextMatch();
            }

            return result;
        }
        private string RemoveNewlines(string input)
        {
            string pattern = @"\n{3,}";
            return Regex.Replace(input, pattern, "\n\n");
        }
    }
}
