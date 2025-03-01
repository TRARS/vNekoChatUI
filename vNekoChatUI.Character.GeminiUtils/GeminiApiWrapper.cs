using Common.Extensions;
using Common.WebWpfCommon;
using Common.WPF;
using Common.WPF.Services;
using GenerativeAI;
using GenerativeAI.Types;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace vNekoChatUI.Character.GeminiUtils
{
    public partial class GeminiApiWrapper
    {
        GeminiApiClient _client = GeminiApiClient.Instance;
    }

    public partial class GeminiApiWrapper
    {
        IFlagService _flagService = ServiceHost.Instance.GetService<IFlagService>();

        public async Task<string> UserSay(string inputs, CancellationToken cancellationToken, Action<int> stepUp)
        {
            stepUp.Invoke(1);

            var generateContentRequest = new GenerateContentRequest();
            var chatHistory = new List<Content>();

            //1. 解析JSON（如果传入数据格式不对，在这里就寄了）
            var jsonObject = JsonSerializer.Deserialize<InputData>(inputs, new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            var ai_name = jsonObject!.Ai_Name;
            var ai_profile = jsonObject.Ai_Profile;
            var ai_content = jsonObject.Ai_Content;
            var ai_innermonologue = jsonObject.Ai_InnerMonologue;// 读取【gemini_model】
            var ai_continuePrompt = jsonObject.Ai_ContinuePrompt;// 读取【一次性提示词】用来引导AI角色做事倾向

            //2. 载入AI设定
            if (_flagService.GeminiTrimWhiteSpaceAndNewLine[0])
            {
                ai_profile = jsonObject.TrimWhiteSpaceAndNewLine(ai_profile);
                ai_content.ForEach(x =>
                {
                    x.Content = jsonObject.TrimWhiteSpaceAndNewLine(x.Content);
                });
                //ai_continuePrompt = jsonObject.TrimNewLine(ai_continuePrompt);
                //Debug.WriteLine("已裁剪Gemini Content");
            }

            //3. 载入聊天记录
            if (ai_content is not null)
            {
                foreach (var x in ai_content)
                {
                    var input = x.Content.RemoveComments().Trim();
                    switch (x.Roles)
                    {
                        case "User":
                            chatHistory.Add(RequestExtensions.FormatGenerateContentInput(input, "USER")); break;
                        case "Assistant":
                            chatHistory.Add(RequestExtensions.FormatGenerateContentInput(input, "MODEL")); break;
                    }
                }
            }

            //4. 打包SystemInstruction和ChatHistory
            var content = CreateContentRequest(ai_profile, chatHistory);
            {
                if (!string.IsNullOrWhiteSpace(ai_continuePrompt))
                {
                    var cp = RequestExtensions.FormatGenerateContentInput($"{ai_continuePrompt}", "USER"); Debug.WriteLine(cp);
                    content.Contents = [.. content.Contents, cp];
                }
            }

            return await _client.WaitReplyAsync(ai_name, ai_profile, content, ai_innermonologue, cancellationToken, stepUp);
        }

        /// <summary>
        /// 生成 Request
        /// </summary>
        private GenerateContentRequest CreateContentRequest(string systemInstruction, List<Content> history)
        {
            return new GenerateContentRequest()
            {
                SystemInstruction = RequestExtensions.FormatSystemInstruction(systemInstruction),
                Contents = history,
            };
        }
    }
}
