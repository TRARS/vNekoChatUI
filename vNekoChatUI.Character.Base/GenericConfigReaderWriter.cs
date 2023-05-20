using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace vNekoChatUI.Character.Base
{
    public class IniStruct
    {
        [JsonPropertyName("ChatGptApiKey")]
        public List<string> ChatGptApiKey { get; set; }
        [JsonPropertyName("BingGptCookie")]
        public List<string> BingGptCookie { get; set; }
    }

    public sealed partial class GenericConfigReaderWriter
    {
        private static readonly object objlock = new object();
        private static GenericConfigReaderWriter? _instance;
        public static GenericConfigReaderWriter Instance
        {
            get
            {
                lock (objlock)
                {
                    if (_instance is null)
                    {
                        _instance = new GenericConfigReaderWriter();
                    }
                }
                return _instance;
            }
        }
    }

    public sealed partial class GenericConfigReaderWriter
    {
        private readonly IConfigurationRoot _configuration;
        private readonly string _iniPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "vNekoChatUI.ini");
       
        public GenericConfigReaderWriter()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddIniFile(_iniPath, optional: false);

            _configuration = builder.Build();
        }

        public string GetValue(string section, string key, string defaultValue = null)
        {
            //.Get<IniStruct>()?.ChatGptApiKey ?? defaultValue; ;
            return $"{_configuration.GetSection(section).GetValue<string>(key)}";
        }

        public void SetValue(string section, string key, string value)
        {
            var obj = _configuration.GetSection(section);
            if (obj.Exists())
            {
                // 配置节存在
                foreach (IConfigurationSection child in obj.GetChildren())
                {
                    if (child.Key.Trim() == $"{key}".Trim())
                    {
                        obj.GetSection(child.Key).Value = $"{value}".Trim();
                        break;
                    }
                }
            }
            else
            {
                // 配置节不存在
                _configuration[$"{section}:{key}"] = $"{value}".Trim();
            }
        }

        public void Save()
        {
            using (var fs = new FileStream(_iniPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (var section in _configuration.GetChildren())
                    {
                        sw.WriteLine($"[{section.Path}]");

                        foreach (var property in section.GetChildren())
                        {
                            sw.WriteLine($"{property.Key} = {property.Value}");
                        }

                        sw.WriteLine();
                    }
                }
            }
        }
    }
}
