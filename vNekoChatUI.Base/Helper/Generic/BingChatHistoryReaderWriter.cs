using Common.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace vNekoChatUI.Base.Helper.Generic
{
    //模型
    public record BingChatHistoryModel([property: JsonPropertyName("username")] string Username,
                                       [property: JsonPropertyName("userbordercolor")] string UserborderColor,
                                       [property: JsonPropertyName("imagesource")] string ImageSource,
                                       [property: JsonPropertyName("message")] string Message,
                                       [property: JsonPropertyName("time")] DateTime Time,
                                       [property: JsonPropertyName("isbot")] bool IsBot);
}

namespace vNekoChatUI.Base.Helper.Generic
{
    //单例
    public sealed partial class BingChatHistoryReaderWriter
    {
        private static readonly object objlock = new object();
        private static BingChatHistoryReaderWriter? _instance;
        public static BingChatHistoryReaderWriter Instance
        {
            get
            {
                lock (objlock)
                {
                    if (_instance is null)
                    {
                        _instance = new BingChatHistoryReaderWriter();
                    }
                }
                return _instance;
            }
        }
    }

    //构造
    public sealed partial class BingChatHistoryReaderWriter
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true,
            WriteIndented = true,
        };
    }

    //公开
    public sealed partial class BingChatHistoryReaderWriter
    {
        public void SaveBingChatHistory(List<string> _list)
        {
            if (_list.Count == 0) { return; }

            List<BingChatHistoryModel> _temp = new();
            {
                _list.ForEach(item =>
                {
                    var jsonObj = JsonSerializer.Deserialize<BingChatHistoryModel>(item, _options);
                    _temp.Add(jsonObj!);
                });
            }
            var result = JsonSerializer.Serialize(_temp, _options);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, "_messages.txt");
            //LogProxy.Instance.Print(result);
            File.WriteAllText(filePath, result);
        }


        public IEnumerable<BingChatHistoryModel> LoadBingChatHistory(string _filepath)
        {
            List<BingChatHistoryModel>? list = null;
            try
            {
                var text = File.ReadAllText(_filepath);
                list = JsonSerializer.Deserialize<List<BingChatHistoryModel>>(text, _options);
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"LoadBingChatHistory Error: {ex.Message}");
            }

            if (list?.Count > 0)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
        }
    }
}
