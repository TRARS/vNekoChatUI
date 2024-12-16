using Common.WPF;
using Common.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using vNekoChatUI.Character.ChatGptUtils.Extensions;

namespace vNekoChatUI.Character.ChatGptUtils
{
    //限制为单例
    public sealed partial class ChatGptApiClient
    {
        private static readonly Lazy<ChatGptApiClient> lazyObject = new(() => new ChatGptApiClient());
        public static ChatGptApiClient Instance => lazyObject.Value;
    }

    //拉一下服务
    public sealed partial class ChatGptApiClient
    {
        IDefWebService _defWebService = ServiceHost.Instance.GetService<IDefWebService>();
        IJsonConfigManagerService _jsonConfigManagerService = ServiceHost.Instance.GetService<IJsonConfigManagerService>();
    }

    //初始化
    public sealed partial class ChatGptApiClient
    {
        //static string api_key => IniConfigReaderWriter.Instance.GetValue("ChatGPT", "api_key");
        string _api_key => _jsonConfigManagerService.GetChatGptApiKey();

        HttpClient _client;

        public ChatGptApiClient()
        {
            // Microsoft DependancyInjection
            var service = new ServiceCollection();
            {
                service.AddHttpClient(string.Empty, httpClient =>
                {
                    //                   
                }).ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler()
                    {
                        Proxy = _defWebService.GetWebProxy(),
                        //UseProxy = true,
                    };

                    if (true)
                    {
                        //サーバ証明書のエラーを無視する方法
                        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    }

                    return handler;
                });
            }
            var provider = service.BuildServiceProvider();

            // Microsoft Http クライアントファクトリーを得る
            var factory = provider.GetRequiredService<IHttpClientFactory>();

            // Http クライアントを得る
            _client = factory.CreateClient();
            _client.Timeout = TimeSpan.FromSeconds(15);//超时
        }
    }


    //内部
    public sealed partial class ChatGptApiClient
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private async Task<ChatGPTResponse> SendToChatGPT(List<ChatGPTMessage> _messages, string _currentApiKey)
        {
            // 格式化
            var chatgpt_request = new ChatGPTRequest("gpt-3.5-turbo", _messages) { };
            var history = JsonSerializer.Serialize(chatgpt_request, jsonOptions);

            // Http リクエストを用意
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            {
                var content = new StringContent(history, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", $"Bearer {_currentApiKey}");
                request.Content = content;

                //LogProxy.Instance.Print(api_key);
                LogProxy.Instance.Print($"Upload: {history}");
            }

            #region 访问外网测试（已禁用
            //bool ConnectionFlag = false;
            //for (int retryCount = 0; retryCount < 20; retryCount++)
            //{
            //    try
            //    {
            //        var response = await _client.GetAsync("https://platform.openai.com/docs/api-reference");
            //        response.EnsureSuccessStatusCode();
            //        LogProxy.Instance.Print("Pre Connection successful.");

            //        ConnectionFlag = true; break;
            //    }
            //    catch (Exception ex)
            //    {
            //        var error_msg = $"Pre Connection failed: {ex.Message}";
            //        if (ex.InnerException != null)
            //        {
            //            error_msg += $"\n——Inner exception: {ex.InnerException.Message}";
            //        }
            //        LogProxy.Instance.Print(error_msg);

            //        await Task.Delay(500);
            //        continue;
            //    }
            //}

            ////强制返回错误
            //return new ChatGPTResponse() { Flag = false };
            #endregion

            //上述测试通过后再进行ChatGPT对话（不再需要上述测试）
            if (true)
            {
                //重试个两三次
                for (int retryCount = 0; retryCount < 5; retryCount++)
                {
                    // Cancelable
                    CancellationTokenSource cancellationTokenSource = new();

                    // Http アクセス
                    HttpResponseMessage response;
                    try
                    {
                        var request_clone = await request.Clone();//必须克隆，否则retry时会报错"禁重复请求"

                        response = await _client.SendAsync(request_clone, cancellationTokenSource.Token);

                        // 成功したら内容(HTML)を表示する
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = await response.Content.ReadAsStringAsync();
                            LogProxy.Instance.Print($"Download: {responseString}");

                            {
                                var obj = JsonSerializer.Deserialize<ChatGPTResponse>(responseString, jsonOptions);

                                if (obj is not null)
                                {
                                    obj.Flag = true;
                                    return obj;//ここまで辿り着ければもうおっけだ
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Unexpected response: {responseString}");
                                }
                            }
                        }
                        else
                        {
                            //string responseString = await response.Content.ReadAsStringAsync();
                            LogProxy.Instance.Print($"response failed: {response.StatusCode}");
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

                LogProxy.Instance.Print($"ChatGptApi retry limit exceeded.");
            }

            return new ChatGPTResponse() { Flag = false };
        }
    }

    //公开
    public sealed partial class ChatGptApiClient
    {
        internal async Task<string> WaitReplyAsync(List<ChatGPTMessage> content)
        {
            var currentApiKey = _api_key;

            if (string.IsNullOrWhiteSpace(currentApiKey))
            {
                return new Ai_Response()
                {
                    TotalTokens = int.MinValue,
                    Message = "Error: ChatGPT API Key has not been set yet."
                }.GetJsonStr();
            }

            string result = new Ai_Response()
            {
                TotalTokens = int.MinValue,
                Message = "Ai_Response, Message: Error"
            }.GetJsonStr();

            var response = await SendToChatGPT(content, currentApiKey);
            if (response is not null && response.Flag is true)
            {
                var assistant_message = response.Choices!.First().Message;//AI的回答
                var total_tokens = response.Usage!.TotalTokens;           //本次交互消耗token量

                //格式化一下
                var response_object = new Ai_Response()
                {
                    TotalTokens = total_tokens,
                    Message = $"{assistant_message.Content}"
                };
                var response_json = JsonSerializer.Serialize(response_object, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                });

                result = response_json;
            }

            return result;
        }
    }
}
