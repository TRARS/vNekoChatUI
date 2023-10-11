using Common.WebWpfCommon;
using Common.WPF;
using Common.WPF.Services;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using vNekoChatUI.Character.BingUtils.Models;
using vNekoChatUI.Character.BingUtils.Services;

namespace vNekoChatUI.Character.BingUtils
{
    //��һ�·���
    public partial class BingGptApiWapper
    {
        IFlagService _flagService = ServiceHost.Instance.GetService<IFlagService>();
        IJsonConfigManagerService _jsonConfigManagerService = ServiceHost.Instance.GetService<IJsonConfigManagerService>();
    }

    public partial class BingGptApiWapper
    {
        string _cookie => _jsonConfigManagerService.GetBingGptCookie(_flagService.TryUseBingRandomCookie[0]);

        BingConversation? _bingConversation;//

        //�²㸺���BingGptͨ�ŵ�HttpClien����
        BingGptApiClient _client = BingGptApiClient.Instance;

        //
        Action<int>? _stepUp;
    }

    //
    public partial class BingGptApiWapper
    {
        int invocation_id = 0;

        public async Task<string> UserSay(string inputs, CancellationToken cancellationToken, Action<int> stepUp)
        {
            var currentCookie = _cookie;

            _stepUp = stepUp;

            //���ڲ���cookieҲ�ܷ��ʣ���ֻ��������ʮ���ظ������Ի����б�Ҫ��㼸��cookie
            //if (string.IsNullOrWhiteSpace(currentCookie))
            //{
            //    return new Bing_Response()
            //    {
            //        TotalTokens = 0,
            //        Message = "Error: Bing Cookie has not been set yet."
            //    }.GetJsonStr();
            //}
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

            //1. �û�����
            var user_say = $"{ai_content.LastOrDefault()?.Content}";

            //2. ����ÿ�仰�����ммУ��ǲ�����������Ľ�����һ�ֶԻ�
            //   �����ʹ��ǿ�ƿ����¶Ի���ÿ�α��ж�Ҫ�Ҽ����·��;ͺܷ�������
            jsonObject.History_Changed = true;

            //3. ���������ˣ�ʵ�����ڵִ�����֮ǰ�Ի���ͱ����ϼ��ٻ��ˣ�
            if (invocation_id >= 30) { invocation_id = 0; }

            //4. ����ϲ��޸��������¼,����������
            if ((jsonObject.History_Changed || jsonObject.Ai_Content.Count == 0))
            {
                invocation_id = 0;
            }

            //5. ����ϲ��޸��������¼�����_bingConversation��δʵ�����������»�ȡ BingConversation
            if (invocation_id == 0 || _bingConversation is null)
            {
                _stepUp?.Invoke(1);//�Ʋ���
                LogProxy.Instance.Print($"\n�� step 1: await _client.CreateBingConversation (cookie={currentCookie.Substring(0, 8)}~~~)\n");
                //�¶Ի�
                _bingConversation = await _client.CreateBingConversation(currentCookie, cancellationToken);
                if (_bingConversation is null || _bingConversation.Flag == false)
                {
                    return new Bing_Response()
                    {
                        TotalTokens = 0,
                        Message = "Get BingConversation Error"
                    }.GetJsonStr();
                }
            }

            LogProxy.Instance.Print($"\n��current invocation_id = {invocation_id})\n");

            //6. ���޴�������
            for (int retryCount = 0; retryCount < 3; retryCount++)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested) { break; }

                    _stepUp?.Invoke(2);//�Ʋ���
                    LogProxy.Instance.Print($"\n�� step 2: await _client.WaitReplyAsync (cookie={currentCookie.Substring(0, 8)}~~~)\n");
                    var response = await _client.WaitReplyAsync(new Models.BingRequest($"{user_say}")
                    {
                        Session = new Models.ConversationSession(invocation_id,
                                                                 _bingConversation.ConversationId,
                                                                 _bingConversation.ClientId,
                                                                 _bingConversation.ConversationSignature,
                                                                 _bingConversation.Encryptedconversationsignature),
                        InputData = jsonObject//�����¼ֱ������ȥ
                    }, cancellationToken, _stepUp, currentCookie);

                    //
                    invocation_id++;
                    return new Bing_Response()
                    {
                        TotalTokens = response?.TotalTokens ?? -1,
                        Message = response?.Message ?? "BingGptApi null response"
                    }.GetJsonStr();
                }
                catch (Exception ex)
                {
                    LogProxy.Instance.Print($"BingChatApiWapper.UserSay error (retry {retryCount}): {ex.Message}");
                    await Task.Delay(1000);
                }
            }

            return new Bing_Response()
            {
                TotalTokens = 0,
                Message = $"BingGptApi retry limit exceeded."
            }.GetJsonStr();
        }


    }
}
