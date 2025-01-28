using System.Text.Json.Serialization;

namespace Common.WebWpfCommon
{
    /// <summary>
    /// 上层UI传递聊天记录之类的数据给下层时用到的类型
    /// </summary>
    public class InputData
    {
        [JsonPropertyName("ai_name")]
        public string Ai_Name { get; set; } = string.Empty;

        [JsonPropertyName("ai_profile")]
        public string Ai_Profile { get; set; } = string.Empty;

        [JsonPropertyName("ai_content")]
        public List<Ai_Content> Ai_Content { get; set; } = new();

        //以下Bing专用
        [JsonPropertyName("ai_innermonologue")]
        public string Ai_InnerMonologue { get; set; } = string.Empty;

        [JsonPropertyName("ai_continueprompt")]
        public string Ai_ContinuePrompt { get; set; } = string.Empty;

        [JsonPropertyName("history_changed")]
        public bool History_Changed { get; set; } = false;

        [JsonPropertyName("bypass_detection")]
        public bool Bypass_Detection { get; set; } = false;

        //Gemini用
        public string TrimWhiteSpaceAndNewLine(string input)
        {
            return input.Trim().Replace(" ", "").Replace("\\r", "").Replace("\\n", "");
        }
        public string TrimNewLine(string input)
        {
            return input.Trim().Replace("\\r", "").Replace("\\n", "");
        }
        public string TrimSpace(string input)
        {
            return input.Trim().Replace(" ", "");
        }
    };

    //构造完整上下文用
    public class Ai_Content
    {
        [JsonPropertyName("roles")]
        public string Roles { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        //删除用Flag（wpf和web交互时才用到）
        [JsonPropertyName("remove")]
        public bool Remove { get; set; } = false;
    }
}
