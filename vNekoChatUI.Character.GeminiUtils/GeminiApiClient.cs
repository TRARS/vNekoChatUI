using Common.WPF;
using Common.WPF.Services;
using GenerativeAI.Models;
using GenerativeAI.Types;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace vNekoChatUI.Character.GeminiUtils
{
    public sealed partial class GeminiApiClient
    {
        private static readonly Lazy<GeminiApiClient> lazyObject = new(() => new GeminiApiClient());
        public static GeminiApiClient Instance => lazyObject.Value;
    }

    public sealed partial class GeminiApiClient
    {
        IJsonConfigManagerService _jsonConfigManagerService = ServiceHost.Instance.GetService<IJsonConfigManagerService>();

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
    }

    public sealed partial class GeminiApiClient
    {
        string apikey => _jsonConfigManagerService.GetGeminiApiKey();
        string? currentSystemInstruction = null;
        string currentApiKey = "";
        string currentGeminiModel = "";
        bool tryGetNextKey = true;

        internal async Task<string> WaitReplyAsync(string? systemInstruction, GenerateContentRequest request, string geminiModel, CancellationToken cancellationToken)
        {
            var retryLimit = 1; //retryTag:
            var result = "";
            var flag = false;

            do
            {
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
                    // gemini-exp-1121        不好使，违规内容会被夹
                    model = new GenerativeModel(currentApiKey, currentGeminiModel, systemInstruction: currentSystemInstruction);
                    model.SafetySettings = safetySetting;

                    var line = "------------------------";
                    Debug.WriteLine($"{line}\n({currentGeminiModel}) ({currentApiKey})\nSystemInstruction:\n{currentSystemInstruction}\n{line}");
                }

                try
                {
                    //request.SystemInstruction = RequestExtensions.FormatSystemInstruction($"{systemInstruction}");
                    var response = await model.GenerateContentAsync(request, cancellationToken);
                    result = $"{response?.Text()?.Trim()}";
                }
                catch (Exception ex)
                {
                    var errorJsonText = GetErrorJsonText(ex.Message);
                    var errorJsonObj = GetErrorJsonObj(errorJsonText);

                    // 如果达到限额，计数器+1
                    if (errorJsonObj is not null)
                    {
                        // RESOURCE_EXHAUSTED
                        if (errorJsonObj.error.code == 429)
                        {
                            tryGetNextKey = true;  //复位
                            //_ = Task.Run(() =>
                            //{
                            //    System.Windows.MessageBox.Show("接下来会尝试切换到下一个ApiKey");
                            //});

                            if (retryLimit-- > 0)
                            {
                                flag = true;
                                await Task.Delay(1000, cancellationToken); // 1秒后自动重试？
                                //goto retryTag;          // 允许重试次数内，跳转至方法开头
                            }
                        }
                    }

                    result = $"リクエストエラー: {ex.Message}";
                }
            }
            while (flag);

            return result;
        }
    }

    public sealed partial class GeminiApiClient
    {
        private class Error
        {
            /// <summary>
            /// 
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
        }
        private class ErrorJson
        {
            /// <summary>
            /// 
            /// </summary>
            public Error error { get; set; }
        }

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
        private ErrorJson? GetErrorJsonObj(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) { return null; }

            var jsonObject = JsonSerializer.Deserialize<ErrorJson>(input, new JsonSerializerOptions()
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
