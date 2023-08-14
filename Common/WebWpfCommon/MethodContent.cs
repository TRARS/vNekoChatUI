using System.Text.Json.Serialization;

namespace Common.WebWpfCommon
{
    /// <summary>
    /// wpf和web交互时用到的类型
    /// </summary>
    public class MethodContent
    {
        [JsonPropertyName("connectionId")]
        public string ConnectionId { get; set; } = string.Empty;

        [JsonPropertyName("methodname")]
        public string MethodName { get; set; } = string.Empty;

        [JsonPropertyName("methodpara")]
        public string MethodPara { get; set; } = string.Empty;
    }
}
