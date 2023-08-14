using Common.WPF;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace vNekoChatUI.Character.SocketUtils.Service
{
    public interface IJsonService
    {
        public T? JsonDeserialize<T>(string jsonText);
        public string JsonSerialize<T>(T jsonObject);
    }

    public class JsonService : IJsonService
    {
        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public T? JsonDeserialize<T>(string jsonText)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(jsonText, options);
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"JsonDeserialize Error—{ex.Message}");
                return default;
            }
        }

        public string JsonSerialize<T>(T jsonObject)
        {
            return JsonSerializer.Serialize(jsonObject, options);
        }
    }
}
