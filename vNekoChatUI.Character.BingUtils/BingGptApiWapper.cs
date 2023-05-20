using Common;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using vNekoChatUI.Base.Helper.Generic;
using vNekoChatUI.Character.BingUtils.Models;
using vNekoChatUI.Character.BingUtils.Services;

namespace vNekoChatUI.Character.BingUtils
{
    public partial class BingGptApiWapper
    {
        //static string cookie => IniConfigReaderWriter.Instance.GetValue("BingGPT", "cookie");
        string _cookie => JsonConfigReaderWriter.Instance.GetBingGptCookie();

        CancellationTokenSource chatCts = new();

        BingConversation? _bingConversation;//

        //下层负责和BingGpt通信的HttpClien对象
        BingGptApiClient _client = BingGptApiClient.Instance;
    }

    //
    public partial class BingGptApiWapper
    {
        int invocation_id = 0;

        public async Task<string> UserSay(string inputs)
        {
            var currentCookie = _cookie;

            if (string.IsNullOrWhiteSpace(currentCookie))
            {
                return "Error: Bing Cookie has not been set yet.";
            }
            //
            var jsonObject = JsonSerializer.Deserialize<InputData>(inputs, new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            //var ai_name = jsonObject!.Ai_Name;
            //var ai_profile = jsonObject.Ai_Profile;
            var ai_content = jsonObject.Ai_Content;

            //1. 用户发言
            var user_say = $"{ai_content.LastOrDefault()?.Content}";

            //2. 反正每句话都被夹夹夹，那不如带着上下文进入新一轮对话
            //   如果不使用强制开启新对话，则需要管理 invocation_id，不然会各种报错
            jsonObject.History_Changed = true;

            //3. 计数器按用户发言数量确定？UI层可以随便删除记录，不太好处理
            //invocation_id = Math.Max(0, jsonObject.Ai_Content.Where(x => x.Roles == "User").Count() - 1);

            //4. 如果上层修改了聊天记录,计数器归零
            if (jsonObject.History_Changed || jsonObject.Ai_Content.Count == 0)
            {
                invocation_id = 0;
            }

            //5. 如果上层修改了聊天记录（或计_bingConversation尚未实例化）则重新获取 BingConversation
            if (invocation_id == 0 || _bingConversation is null)
            {
                //新对话
                _bingConversation = await _client.WaitReplyAsync(currentCookie);
                if (_bingConversation is null || _bingConversation.Flag == false)
                {
                    return "Get BingConversation Error";
                }
            }

            LogProxy.Instance.Print($"\n（current invocation_id = {invocation_id})\n");

            //6. 有限次数重试
            for (int retryCount = 0; retryCount < 3; retryCount++)
            {
                try
                {
                    var response = await _client.ChatAsync(new Models.BingRequest($"{user_say}")
                    {
                        Session = new Models.ConversationSession(invocation_id++,
                                                                 _bingConversation.ConversationId,
                                                                 _bingConversation.ClientId,
                                                                 _bingConversation.ConversationSignature),
                        InputData = jsonObject//聊天记录直接塞进去
                    }, chatCts.Token);

                    //LogProxy.Instance.Print($"Bing-Bot Response: {response}");
                    return $"{response}";
                }
                catch (Exception ex)
                {
                    LogProxy.Instance.Print($"BingChatApiWapper.UserSay error (retry {retryCount}): {ex.Message}");
                    await Task.Delay(1000);
                }
            }

            return $"BingGptApi retry limit exceeded.";
        }


    }
}
