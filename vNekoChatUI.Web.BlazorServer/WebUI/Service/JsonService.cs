using System.Text.Json;
using System.Text.Json.Serialization;

namespace vNekoChatUI.Web.BlazorServer.WebUI.Service
{
    public class JsonService
    {
        public string Serialize(dynamic content, bool writeIndented = false)
        {
            var jsonStr = JsonSerializer.Serialize(content, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = writeIndented
            });

            return jsonStr;
        }

        public T? Deserialize<T>(string json)
        {
            var jsonObj = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                //WriteIndented = true
            });

            return jsonObj;
        }
    }
}
