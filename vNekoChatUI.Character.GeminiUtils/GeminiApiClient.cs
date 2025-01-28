using Common.WPF;
using Common.WPF.Services;
using CommunityToolkit.Mvvm.Messaging;
using GenerativeAI.Models;
using GenerativeAI.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using TrarsUI.Shared.Messages;

namespace vNekoChatUI.Character.GeminiUtils
{
    public sealed partial class GeminiApiClient
    {
        private static readonly Lazy<GeminiApiClient> lazyObject = new(() => new GeminiApiClient());
        public static GeminiApiClient Instance => lazyObject.Value;
    }

    public sealed partial class GeminiApiClient
    {
        IDefWebService _defWebService = ServiceHost.Instance.GetService<IDefWebService>();
        IJsonConfigManagerService _jsonConfigManagerService = ServiceHost.Instance.GetService<IJsonConfigManagerService>();
        IStreamService _streamService = ServiceHost.Instance.GetService<IStreamService>();

        private GenerativeModel model;
        private SafetySetting[] safetySetting =
            [
                //new SafetySetting()
                //{
                //    Category = HarmCategory.HARM_CATEGORY_UNSPECIFIED,
                //    Threshold = HarmBlockThreshold.BLOCK_ONLY_HIGH
                //},
                new SafetySetting()
                {
                    Category = HarmCategory.HARM_CATEGORY_HARASSMENT,
                    Threshold = HarmBlockThreshold.BLOCK_NONE
                },
                new SafetySetting()
                {
                    Category = HarmCategory.HARM_CATEGORY_HATE_SPEECH,
                    Threshold = HarmBlockThreshold.BLOCK_NONE
                },
                new SafetySetting()
                {
                    Category = HarmCategory.HARM_CATEGORY_SEXUALLY_EXPLICIT,
                    Threshold = HarmBlockThreshold.BLOCK_NONE
                },
                new SafetySetting()
                {
                    Category = HarmCategory.HARM_CATEGORY_DANGEROUS_CONTENT,
                    Threshold = HarmBlockThreshold.BLOCK_NONE
                }
            ];

        HttpClient _client;

        public GeminiApiClient()
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
            _client.Timeout = TimeSpan.FromSeconds(45);//超时
        }
    }

    public sealed partial class GeminiApiClient
    {
        string apikey => _jsonConfigManagerService.GetGeminiApiKey();
        string? currentSystemInstruction = null;
        string currentApiKey = "";
        string currentGeminiModel = "";
        bool tryGetNextKey = true;

        internal async Task<string> WaitReplyAsync(string ai_name, string? systemInstruction, GenerateContentRequest request, string geminiModel, CancellationToken cancellationToken, Action<int> stepUp)
        {
            stepUp.Invoke(2);

            var retryLimit = 1;
            var result = "";
            var totalTokenCount = -1;
            var flag = false;

            var fullstr = "";
            var action = new Action<string>(s =>
            {
                fullstr += s;
                _streamService.StartStreamingText(ai_name, fullstr);
                WeakReferenceMessenger.Default.Send(new AlertMessage($"Gemini AI Streaming..."));

                stepUp.Invoke(4);
            });

            do
            {
                stepUp.Invoke(3);
                flag = false;

                // 获得key并储存
                if (string.IsNullOrWhiteSpace(currentApiKey) || tryGetNextKey)
                {
                    currentApiKey = apikey; // 获取一次key
                }

                if (currentSystemInstruction != systemInstruction || tryGetNextKey || currentGeminiModel != geminiModel)
                {
                    currentSystemInstruction = systemInstruction;
                    currentGeminiModel = geminiModel;
                    tryGetNextKey = false;

                    // gemini-1.5-pro-latest  好
                    // gemini-2.0-flash-exp   不太行
                    model = new GenerativeModel(currentApiKey, currentGeminiModel, _client, systemInstruction: currentSystemInstruction);
                    model.SafetySettings = safetySetting;

                    var line = "------------------------";
                    //Debug.WriteLine($"{line}\n({currentGeminiModel}) ({currentApiKey})\nSystemInstruction:\n{currentSystemInstruction}\n{line}");
                }

                try
                {
                    //var response = await model.GenerateContentAsync(request, cancellationToken);
                    //result = $"{response?.Text()?.Trim()}";
                    //totalTokenCount = response?.UsageMetadata?.TotalTokenCount ?? 0;
                    //WeakReferenceMessenger.Default.Send(new AlertMessage($"Gemini AI Response"));

                    var response = await model.StreamContentAsync(request, action, cancellationToken);
                    result = $"{response?.Trim()}";
                    WeakReferenceMessenger.Default.Send(new AlertMessage($"Gemini AI Response"));
                }
                catch (Exception ex)
                {
                    var errorJsonText = GetErrorJsonText(ex.Message);
                    var errorJsonObj = GetErrorJsonObj<GeminiErrorJson>(errorJsonText);

                    // 如果达到限额，计数器+1
                    if (errorJsonObj is not null)
                    {
                        // RESOURCE_EXHAUSTED
                        if (errorJsonObj.error.code == 429)
                        {
                            tryGetNextKey = true;  //复位
                            WeakReferenceMessenger.Default.Send(new AlertMessage("尝试切换到下一个ApiKey"));

                            if (retryLimit-- > 0)
                            {
                                flag = true;
                                await Task.Delay(1000, cancellationToken); // 1秒后自动重试，最多重试 retryLimit 次
                            }
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(new AlertMessage($"GeminiError: {errorJsonObj.error.code}"));
                        }
                    }

                    result = $"リクエストエラー: {ex.Message}";
                }
            }
            while (flag);

            stepUp.Invoke(5);
            stepUp.Invoke(6);

            return (new Gemini_Response()
            {
                Message = result,
                TotalTokens = totalTokenCount
            }).GetJsonStr();
        }
    }

    public sealed partial class GeminiApiClient
    {
        private string GetErrorJsonText(string input)
        {
            string result = string.Empty;
            string pattern = @"\{[\s\S]*\}";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                string json = match.Value;
                //Debug.WriteLine($"提取的JSON文本：\n{json}");
                result = json;
            }
            else
            {
                //Debug.WriteLine("未找到JSON文本");
            }

            return result;
        }
        private T? GetErrorJsonObj<T>(string input) where T : class
        {
            if (string.IsNullOrWhiteSpace(input)) { return null; }

            var jsonObject = JsonSerializer.Deserialize<T>(input, new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            return jsonObject;
        }
    }
}
