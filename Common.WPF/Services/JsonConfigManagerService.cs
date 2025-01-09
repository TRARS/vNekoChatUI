using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.WPF.Services
{
    public interface IJsonConfigManagerService
    {
        public string GetChatGptApiKey();
        public string GetBingGptCookie(bool random_cookie = false);
        public string GetGeminiApiKey();
        public void AddChatGptApiKey(string key);
        public void AddBingGptCookie(string cookie);
        public void AddGeminiApiKey(string key);
        public dynamic GetCurrentChatGptApiKeys();
        public dynamic GetCurrentBingGptCookies();
        public dynamic GetCurrentGeminiApiKeys();
        public void Clear();
        public void SaveToDesktop();

        public string? LoadProfileFromDefaultPath();
        public string? LoadContinuePromptFromDefaultPath();
    }

    //内部类
    public partial class JsonConfigManagerService : IJsonConfigManagerService
    {
        //可观察
        public partial class ObservableString : ObservableObject
        {
            private string _value = string.Empty;
            public string Value
            {
                get { return _value; }
                set
                {
                    SetProperty(ref _value, value.Trim());
                }
            }

            [ObservableProperty]
            private bool _isChecked = true;

            [ObservableProperty]
            private string _tag = string.Empty;



            [property: JsonIgnore]
            [ObservableProperty]
            private int _num = 0;
        }

        //模型
        public class JsonConfigStruct
        {
            [JsonPropertyName("ChatGptApiKey")]
            public ObservableCollection<ObservableString> ChatGptApiKeys { get; init; } = new();

            [JsonPropertyName("BingGptCookie")]
            public ObservableCollection<ObservableString> BingGptCookies { get; init; } = new();

            [JsonPropertyName("GeminiApiKey")]
            public ObservableCollection<ObservableString> GeminiApiKeys { get; init; } = new();

            [JsonPropertyName("LastUsedGeminiApiKey")]
            public string LastUsedGeminiApiKey { get; set; } = string.Empty;

            [JsonPropertyName("DefProfilePath")]
            public string DefProfilePath { get; set; } = string.Empty;
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
        private int currentGeminiApiKeyIndex = 0;

        private string defaultProfilePath = string.Empty;

        public JsonConfigManagerService()
        {
            try
            {
                if (File.Exists(_jsonPath))
                {
                    string jsonString = File.ReadAllText(_jsonPath);
                    _configModel = JsonSerializer.Deserialize<JsonConfigStruct>(jsonString, _options) ?? new();
                    this.RemoveEmpty();
                    this.SetLastUsedGeminiApiKeyIndex(_configModel.LastUsedGeminiApiKey);

                    defaultProfilePath = _configModel.DefProfilePath;
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
                        GeminiApiKeys = new()
                        {
                            new() { Value = "Enter Your Gemini API Key" }
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

            for (int i = _configModel.GeminiApiKeys.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(_configModel.GeminiApiKeys[i].Value))
                {
                    _configModel.GeminiApiKeys.RemoveAt(i);
                }
            }
        }
        //设置GeminiApiKeyIndex
        private void SetLastUsedGeminiApiKeyIndex(string lastApiKey)
        {
            var checkedApiKeyList = _configModel.GeminiApiKeys.Where(s => string.IsNullOrWhiteSpace(s.Value) is false && s.IsChecked).ToList();
            currentGeminiApiKeyIndex = checkedApiKeyList.FindIndex(x => x.Value == lastApiKey);
            if (currentGeminiApiKeyIndex < 0) { currentGeminiApiKeyIndex = 0; } // 没找到就归零
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
            bool hasNonEmptyString = checkedCookieList.Count > 0;

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

        //
        //首个gemini api key
        private string FirstGeminiApiKey()
        {
            return _configModel.GeminiApiKeys.FirstOrDefault()?.Value ?? "";
        }
        //轮换gemini api key
        private string NextGeminiApiKey()
        {
            var checkedApiKeyList = _configModel.GeminiApiKeys.Where(s => string.IsNullOrWhiteSpace(s.Value) is false && s.IsChecked).ToList();
            bool hasNonEmptyString = checkedApiKeyList.Count > 0;

            if (hasNonEmptyString)
            {
                if (currentGeminiApiKeyIndex >= checkedApiKeyList.Count)
                {
                    currentGeminiApiKeyIndex = 0;
                }

                //var key = checkedApiKeyList[currentGeminiApiKeyIndex++];
                //key.Num++;
                var key = checkedApiKeyList[currentGeminiApiKeyIndex];
                Debug.WriteLine($"Fetching API Key: {key.Value}, Index = {currentGeminiApiKeyIndex}");
                currentGeminiApiKeyIndex++; // 索引自增
                Debug.WriteLine($"After fetching: Index = {currentGeminiApiKeyIndex}");
                key.Num++;

                return key.Value ?? "";
            }

            return "empty apikey";
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

        /// <summary>
        /// 每次使用相同 gemini api key 直至额度枯竭
        /// </summary>
        public string GetGeminiApiKey()
        {
            var result = NextGeminiApiKey();
            if (result.Trim().Length == 0)
            {
                return GetGeminiApiKey();
            }

            _configModel.LastUsedGeminiApiKey = result; SaveToDesktop();
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
        public void AddGeminiApiKey(string key)
        {
            _configModel.GeminiApiKeys.Add(new() { Value = key });
        }

        public dynamic GetCurrentChatGptApiKeys()
        {
            return _configModel.ChatGptApiKeys;
        }
        public dynamic GetCurrentBingGptCookies()
        {
            return _configModel.BingGptCookies;
        }
        public dynamic GetCurrentGeminiApiKeys()
        {
            return _configModel.GeminiApiKeys;
        }

        public void Clear()
        {
            _configModel.ChatGptApiKeys.Clear();
            _configModel.BingGptCookies.Clear();
            _configModel.GeminiApiKeys.Clear();
        }
        public void SaveToDesktop() => this.SaveConfigToDesktop();

        public string? LoadProfileFromDefaultPath()
        {
            if (!Directory.Exists(defaultProfilePath)) { return null; }

            // 储存结果
            var result = string.Empty;

            // 获取文件夹中的所有 .txt 文件
            var files = Directory.GetFiles(defaultProfilePath, "*.txt");

            // 使用正则表达式筛选出文件名是纯数字的txt文件
            var regex = new Regex(@"^\d+\.txt$");

            // 遍历文件并读取内容
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file); // 获取文件名
                if (regex.IsMatch(fileName))  // 判断文件名是否为纯数字
                {
                    try
                    {
                        string content = File.ReadAllText(file);  // 读取文件内容
                        result += (content + "\n");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"无法读取文件 {fileName}: {ex.Message}");
                    }
                }
            }

            // 返回
            return result.Replace("\\r\\n", "\n").TrimStart().TrimEnd();
        }

        public string? LoadContinuePromptFromDefaultPath()
        {
            if (!Directory.Exists(defaultProfilePath)) { return null; }

            var filePath = Path.Combine(defaultProfilePath, "ContinuePrompt.txt");

            // 检查文件是否存在
            if (File.Exists(filePath))
            {
                try
                {
                    // 读取文件内容
                    return File.ReadAllText(filePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"无法读取文件 ContinuePrompt.txt: {ex.Message}");
                }
            }

            return null;
        }
    }
}
