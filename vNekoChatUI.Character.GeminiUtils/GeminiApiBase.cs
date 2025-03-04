﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace vNekoChatUI.Character.GeminiUtils
{
    public class GeminiError
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
    public class GeminiErrorJson
    {
        /// <summary>
        /// 
        /// </summary>
        public GeminiError error { get; set; }
    }

    public class Gemini_Response
    {
        [JsonPropertyName("totaltokens")]
        public int TotalTokens { get; set; } = int.MinValue;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        public string GetJsonStr()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });
        }
    }
}
