using System.Text.Json.Serialization;

namespace vNekoChatUI.Character.BingUtils.Models
{
    public class BingConversation
    {
        /// <summary>
        /// 手动操作Flag：true正常，false通信失败
        /// </summary>
        [JsonIgnore]
        public bool Flag = false;

        [JsonPropertyName("conversationId")]
        public string ConversationId { get; set; }

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("conversationSignature")]
        public string ConversationSignature { get; set; }

        [JsonPropertyName("result")]
        public Resule? Result { get; set; }
    }

    public class Resule
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}


