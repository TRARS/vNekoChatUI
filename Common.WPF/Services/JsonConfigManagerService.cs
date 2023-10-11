using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Common.WPF.Services.JsonConfigManagerService;

namespace Common.WPF.Services
{
    public interface IJsonConfigManagerService
    {
        public string GetChatGptApiKey();
        public string GetBingGptCookie(bool random_cookie = false);
        public void AddChatGptApiKey(string key);
        public void AddBingGptCookie(string cookie);
        public dynamic GetCurrentChatGptApiKeys();
        public dynamic GetCurrentBingGptCookies();
        public void Clear();
        public void SaveToDesktop();
    }

    //内部类
    public partial class JsonConfigManagerService: IJsonConfigManagerService
    {
        public class NotificationObject : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            public void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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

            private bool _isChecked = true;
            public bool IsChecked
            {
                get { return _isChecked; }
                set
                {
                    _isChecked = value;
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

    //构造
    public partial class JsonConfigManagerService
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

        public JsonConfigManagerService()
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
                    this.SaveConfigToDesktop();
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
    public partial class JsonConfigManagerService
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
            var checkedApiKeyList = _configModel.ChatGptApiKeys.Where(s => string.IsNullOrWhiteSpace(s.Value) is false && s.IsChecked).ToList();
            bool hasNonEmptyString = checkedApiKeyList.Count > 0;

            if (hasNonEmptyString)
            {
                if (currentChatGptApiKeyIndex >= checkedApiKeyList.Count)
                {
                    currentChatGptApiKeyIndex = 0;
                }
                return checkedApiKeyList[currentChatGptApiKeyIndex++].Value ?? "";
            }

            return "empty apikey";
        }

        //首个bing cookie
        private string FirstBingGptCookie()
        {
            return _configModel.BingGptCookies.FirstOrDefault()?.Value ?? "";
        }
        //轮换bing cookie
        private string NextBingGptCookie()
        {
            var checkedCookieList = _configModel.BingGptCookies.Where(s => string.IsNullOrWhiteSpace(s.Value) is false && s.IsChecked).ToList();
            bool hasNonEmptyString = checkedCookieList.Count > 0 ;

            if (hasNonEmptyString)
            {
                if (currentBingGptCookieIndex >= checkedCookieList.Count())
                {
                    currentBingGptCookieIndex = 0;
                }
                return checkedCookieList[currentBingGptCookieIndex++].Value ?? "";
            }

            return "empty cookie";
        }

        //存档
        private void SaveConfigToDesktop()
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

    //公开
    public partial class JsonConfigManagerService
    {
        /// <summary>
        /// 使用首个chatgpt api key
        /// </summary>
        public string GetChatGptApiKey()
        {
            var result = NextChatGptApiKey();
            if (result.Trim().Length == 0)
            {
                return GetChatGptApiKey();
            }
            return result;
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

        public dynamic GetCurrentChatGptApiKeys()
        {
            return _configModel.ChatGptApiKeys;
        }
        public dynamic GetCurrentBingGptCookies()
        {
            return _configModel.BingGptCookies;
        }

        public void Clear()
        {
            _configModel.ChatGptApiKeys.Clear();
            _configModel.BingGptCookies.Clear();
        }
        public void SaveToDesktop() => this.SaveConfigToDesktop();
    }
}
