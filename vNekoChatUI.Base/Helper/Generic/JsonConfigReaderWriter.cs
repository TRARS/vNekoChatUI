using Common.WPF;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace vNekoChatUI.Base.Helper.Generic
{
    //可观察
    public class ObservableString : NotificationObject
    {
        private string _value = string.Empty;
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value.Trim();
                NotifyPropertyChanged();
            }
        }
    }

    //模型
    public class JsonConfigStruct
    {
        [JsonPropertyName("ChatGptApiKey")]
        public ObservableCollection<ObservableString> ChatGptApiKeys { get; init; } = new();

        [JsonPropertyName("BingGptCookie")]
        public ObservableCollection<ObservableString> BingGptCookies { get; init; } = new();
    }
}

namespace vNekoChatUI.Base.Helper.Generic
{
    //单例
    public sealed partial class JsonConfigReaderWriter
    {
        private static readonly object objlock = new object();
        private static JsonConfigReaderWriter? _instance;
        public static JsonConfigReaderWriter Instance
        {
            get
            {
                lock (objlock)
                {
                    if (_instance is null)
                    {
                        _instance = new JsonConfigReaderWriter();
                    }
                }
                return _instance;
            }
        }
    }

    //构造
    public sealed partial class JsonConfigReaderWriter
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true,
            WriteIndented = true,
        };
        private readonly JsonConfigStruct _configModel;
        private readonly string _jsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "vNekoChatUI.json");

        private int currentChatGptApiKeyIndex = 0;
        private int currentBingGptCookieIndex = 0;

        public JsonConfigReaderWriter()
        {
            try
            {
                if (File.Exists(_jsonPath))
                {
                    string jsonString = File.ReadAllText(_jsonPath);
                    _configModel = JsonSerializer.Deserialize<JsonConfigStruct>(jsonString, _options) ?? new();
                    this.RemoveEmpty();
                }
                else
                {
                    _configModel = new()
                    {
                        ChatGptApiKeys = new()
                        {
                            new() { Value = "Enter Your ChatGPT API Key" }
                        },
                        BingGptCookies = new()
                        {
                            new() { Value = "Enter Your Bing Cookie" }
                        },
                    };
                    this.SaveToDesktop();
                }
            }
            catch (Exception ex)
            {
                LogProxy.Instance.Print($"JsonConfigReaderWriter Error—{ex.Message}");
                _configModel = new();
            }
        }
    }

    //内部
    public sealed partial class JsonConfigReaderWriter
    {
        //除空
        private void RemoveEmpty()
        {
            for (int i = _configModel.ChatGptApiKeys.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(_configModel.ChatGptApiKeys[i].Value))
                {
                    _configModel.ChatGptApiKeys.RemoveAt(i);
                }
            }
            for (int i = _configModel.BingGptCookies.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(_configModel.BingGptCookies[i].Value))
                {
                    _configModel.BingGptCookies.RemoveAt(i);
                }
            }
        }

        //首个chatgpt api key
        private string FirstChatGptApiKey()
        {
            return _configModel.ChatGptApiKeys.FirstOrDefault()?.Value ?? "";
        }
        //轮换chatgpt api key
        private string NextChatGptApiKey()
        {
            if (currentChatGptApiKeyIndex >= _configModel.ChatGptApiKeys.Count)
            {
                currentChatGptApiKeyIndex = 0;
            }
            return _configModel.ChatGptApiKeys[currentChatGptApiKeyIndex++].Value ?? "";
        }

        //首个bing cookie
        private string FirstBingGptCookie()
        {
            return _configModel.BingGptCookies.FirstOrDefault()?.Value ?? "";
        }
        //轮换bing cookie
        private string NextBingGptCookie()
        {
            bool hasNonEmptyString = _configModel.BingGptCookies.Any(s => string.IsNullOrWhiteSpace(s.Value) is false);

            if (hasNonEmptyString)
            {
                if (currentBingGptCookieIndex >= _configModel.BingGptCookies.Count)
                {
                    currentBingGptCookieIndex = 0;
                }
                return _configModel.BingGptCookies[currentBingGptCookieIndex++].Value ?? "";
            }

            return "empty cookie";
        }
    }

    //公开
    public sealed partial class JsonConfigReaderWriter
    {
        /// <summary>
        /// 使用首个chatgpt api key
        /// </summary>
        public string GetChatGptApiKey()
        {
            return FirstChatGptApiKey();

            //var result = NextChatGptApiKey();
            //if (result.Trim().Length == 0)
            //{
            //    return GetChatGptApiKey();
            //}
            //return result;
        }

        /// <summary>
        /// 每次使用不同bing cookie
        /// </summary>
        public string GetBingGptCookie(bool random_cookie = false)
        {
            if (random_cookie) { return $"{Guid.NewGuid()}"; }

            var result = NextBingGptCookie();
            if (result.Trim().Length == 0)
            {
                return GetBingGptCookie();
            }
            return result;
        }

        public void AddChatGptApiKey(string key)
        {
            _configModel.ChatGptApiKeys.Add(new() { Value = key });
        }
        public void AddBingGptCookie(string cookie)
        {
            _configModel.BingGptCookies.Add(new() { Value = cookie });
        }

        public ObservableCollection<ObservableString> GetCurrentChatGptApiKeys()
        {
            return _configModel.ChatGptApiKeys;
        }
        public ObservableCollection<ObservableString> GetCurrentBingGptCookies()
        {
            return _configModel.BingGptCookies;
        }

        public void Clear()
        {
            _configModel.ChatGptApiKeys.Clear();
            _configModel.BingGptCookies.Clear();
        }
        public void SaveToDesktop()
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);//给点反应时间
                using (StreamWriter sw = new StreamWriter($"{_jsonPath}", false, Encoding.Unicode))
                {
                    sw.WriteLine(JsonSerializer.Serialize(_configModel, _options));
                }
                LogProxy.Instance.Print("SaveConfigToDesktop done");
            });
        }
    }
}
