using Common.WPF;
using Common.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using vNekoChatUI.Character.BingUtils.Models;

namespace vNekoChatUI.Character.BingUtils.Services
{
    //限制为单例
    public sealed partial class BingGptApiClient
    {
        private static readonly Lazy<BingGptApiClient> lazyObject = new(() => new BingGptApiClient());
        public static BingGptApiClient Instance => lazyObject.Value;
    }

    //拉一下服务
    public sealed partial class BingGptApiClient
    {
        IDefWebService _defWebService = ServiceHost.Instance.GetService<IDefWebService>();
    }

    //初始化
    public sealed partial class BingGptApiClient
    {
        HttpClient _client;

        BingChatClient _bingChatClient;

        public BingGptApiClient()
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
            _client.Timeout = TimeSpan.FromSeconds(10);//超时
        }
    }


    //内部
    public sealed partial class BingGptApiClient
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        //
        private async Task<BingConversation> SendToBingGPT(string cookie, CancellationToken cancellationToken)
        {
            // Http リクエストを用意
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.bing.com/turing/conversation/create");
            {
                request.Headers.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"111\", \"Not(A:Brand\";v=\"8\", \"Chromium\";v=\"111\"");
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                //request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.0.0");
                request.Headers.Add("user-agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Mobile Safari/537.36");
                //request.Headers.Add("content-type", "application/json");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("sec-ch-ua-platform-version", "15.0.0");
                request.Headers.Add("x-ms-client-request-id", Guid.NewGuid().ToString());
                request.Headers.Add("x-ms-useragent", "azsdk-js-api-client-factory/1.0.0-beta.1 core-rest-pipeline/1.10.0 OS/Win32");
                request.Headers.Add("referer", "https://www.bing.com/search?q=Bing+AI&showconv=1&FORM=hpcodx");
                request.Headers.Add("x-forwarded-for", "1.1.1.1");
                request.Headers.Add("cookie", $"_U={cookie}");

                var content = new StringContent("", Encoding.UTF8, "application/json");
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                request.Content = content;
            }

            if (true)
            {
                //重试个两三次
                for (int retryCount = 0; retryCount < 3; retryCount++)
                {
                    if (cancellationToken.IsCancellationRequested) { break; }

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
                            LogProxy.Instance.Print($"Download(len={responseString.Length}): {responseString}");

                            {
                                var obj = JsonSerializer.Deserialize<BingConversation>(responseString, jsonOptions);

                                if (obj is not null)
                                {
                                    //?
                                    if (response.Headers.TryGetValues("X-Sydney-Encryptedconversationsignature", out var values))
                                    {
                                        obj.Encryptedconversationsignature = values?.FirstOrDefault();
                                        if (obj.Encryptedconversationsignature is not null)
                                        {
                                            obj.Encryptedconversationsignature = System.Web.HttpUtility.UrlEncode(obj.Encryptedconversationsignature);
                                        }
                                        //System.Windows.MessageBox.Show($"===\n{obj.ConversationId}\n{obj.Encryptedconversationsignature}\n===");
                                        LogProxy.Instance.Print($"Encryptedconversationsignature: \n{obj.Encryptedconversationsignature}");
                                    }

                                    obj.Flag = true;
                                    return obj;
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

                LogProxy.Instance.Print($"BingGptApi retry limit exceeded.");
            }

            return new BingConversation() { Flag = false };
        }
    }

    //公开
    public sealed partial class BingGptApiClient
    {
        /// <summary>
        /// 返回BingConversation
        /// </summary>
        internal async Task<BingConversation?> CreateBingConversation(string cookie, CancellationToken cancellationToken)
        {
            _bingChatClient = new BingChatClient(new Models.BingChatOption(Enum.CharStyle.Creative, cookie, 1000),
                                                 _client);

            var response = await SendToBingGPT(cookie, cancellationToken);
            if (response is not null && response.Flag is true)
            {
                return response;
            }

            return null;
        }

        internal async Task<Bing_Response?> WaitReplyAsync(BingRequest message, CancellationToken cancellationToken, Action<int> stepUp, string cookie)
        {
            return await _bingChatClient.WaitReplyAsync(message, cancellationToken, stepUp, cookie);
        }
    }
}
