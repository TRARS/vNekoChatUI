using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static vNekoChatUI.Character.BingUtils.Models.NetworkFixedResponse;

namespace vNekoChatUI.Character.BingUtils.Models
{
    //
    public class NetworkFixedResponse
    {
        //储存自身json字符串
        public string MyJsonString { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }
        [JsonPropertyName("target")]
        public string Target { get; set; }
        [JsonPropertyName("arguments")]
        public List<Argument> Arguments { get; set; }

        public class AdaptiveCard
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }
            [JsonPropertyName("version")]
            public string Version { get; set; }
            [JsonPropertyName("body")]
            public List<TextBlock> Body { get; set; }
        }

        public class TextBlock
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }
            [JsonPropertyName("text")]
            public string Text { get; set; }
            [JsonPropertyName("wrap")]
            public bool Wrap { get; set; }
        }

        public class Feedback
        {
            [JsonPropertyName("tag")]
            public object Tag { get; set; }
            [JsonPropertyName("updateOn")]
            public object UpdatedOn { get; set; }
            [JsonPropertyName("type")]
            public string Type { get; set; }
        }

        public class Message
        {
            //被夹了就只有 HiddenText 没有 Text
            [JsonPropertyName("hiddenText")]
            public string? HiddenText { get; set; }

            [JsonPropertyName("text")]
            public string? Text { get; set; }
            [JsonPropertyName("author")]
            public string Author { get; set; }
            [JsonPropertyName("createdAt")]
            public DateTimeOffset CreatedAt { get; set; }
            [JsonPropertyName("timestamp")]
            public DateTimeOffset Timestamp { get; set; }
            [JsonPropertyName("messageId")]
            public string MessageId { get; set; }
            [JsonPropertyName("offense")]
            public string Offense { get; set; }
            [JsonPropertyName("adaptiveCards")]
            public List<AdaptiveCard> AdaptiveCards { get; set; }
            [JsonPropertyName("sourceAttributions")]
            public List<object> SourceAttributions { get; set; }
            [JsonPropertyName("feedback")]
            public Feedback Feedback { get; set; }
            [JsonPropertyName("contentOrigin")]
            public string ContentOrigin { get; set; }
            [JsonPropertyName("privacy")]
            public object Privacy { get; set; }
        }

        public class Argument
        {
            [JsonPropertyName("messages")]
            public List<Message>? Messages { get; set; }
            [JsonPropertyName("requestId")]
            public string RequestId { get; set; }
        }
    }
}




// Type2 使用的结构
namespace vNekoChatUI.Character.BingUtils.Models
{
    public class Type2Response
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }
        [JsonPropertyName("invocationId")]
        public string InvocationId { get; set; }
        [JsonPropertyName("item")]
        public Item_ Item { get; set; }

        public class Item_
        {
            [JsonPropertyName("messages")]
            public List<Message> Messages { get; set; }
            [JsonPropertyName("firstNewMessageIndex")]
            public object FirstNewMessageIndex { get; set; }
            [JsonPropertyName("defaultChatName")]
            public object DefaultChatName { get; set; }
            [JsonPropertyName("conversationId")]
            public string ConversationId { get; set; }
            [JsonPropertyName("requestId")]
            public string RequestId { get; set; }
            [JsonPropertyName("telemetry")]
            public Telemetry Telemetry { get; set; }
            [JsonPropertyName("throttling")]
            public Throttling Throttling { get; set; }
            [JsonPropertyName("result")]
            public Result Result { get; set; }
        }

        public class Message
        {
            //保留必要的
            [JsonPropertyName("hiddenText")]
            public string? HiddenText { get; set; }
            [JsonPropertyName("text")]
            public string? Text { get; set; }
            [JsonPropertyName("author")]
            public string Author { get; set; }
            [JsonPropertyName("suggestedResponses")]
            public List<SuggestedResponse> SuggestedResponses { get; set; }

            [JsonPropertyName("contentOrigin")]
            public string ContentOrigin { get; set; } //DeepLeo //Apology
            [JsonPropertyName("adaptiveCards")]
            public List<AdaptiveCard> AdaptiveCards { get; set; }
        }

        public class Telemetry
        {
            [JsonPropertyName("metrics")]
            public object Metrics { get; set; }
            [JsonPropertyName("startTime")]
            public DateTime StartTime { get; set; }
        }

        public class Throttling
        {
            [JsonPropertyName("maxNumUserMessagesInConversation")]
            public int MaxNumUserMessagesInConversation { get; set; }
            [JsonPropertyName("numUserMessagesInConversation")]
            public int NumUserMessagesInConversation { get; set; }
            [JsonPropertyName("maxNumLongDocSummaryUserMessagesInConversation")]
            public int MaxNumLongDocSummaryUserMessagesInConversation { get; set; }
            [JsonPropertyName("numLongDocSummaryUserMessagesInConversation")]
            public int NumLongDocSummaryUserMessagesInConversation { get; set; }
        }

        public class Result
        {
            [JsonPropertyName("value")]
            public string Value { get; set; }
            [JsonPropertyName("message")]
            public string Message { get; set; }
            [JsonPropertyName("error")]
            public string Error { get; set; }
            [JsonPropertyName("serviceVersion")]
            public string ServiceVersion { get; set; }
        }

        public class SuggestedResponse
        {
            public string text { get; set; }
            public string author { get; set; }
            public string messageType { get; set; }
        }
    }

}
