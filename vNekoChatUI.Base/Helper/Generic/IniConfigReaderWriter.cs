namespace vNekoChatUI.Base.Helper.Generic
{
    //public sealed partial class IniConfigReaderWriter
    //{
    //    private static readonly object objlock = new object();
    //    private static IniConfigReaderWriter? _instance;
    //    public static IniConfigReaderWriter Instance
    //    {
    //        get
    //        {
    //            lock (objlock)
    //            {
    //                if (_instance is null)
    //                {
    //                    _instance = new IniConfigReaderWriter();
    //                }
    //            }
    //            return _instance;
    //        }
    //    }
    //}

    //public sealed partial class IniConfigReaderWriter
    //{
    //    private readonly IConfigurationRoot _configuration;
    //    private readonly string _iniPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "vNekoChatUI.ini");

    //    public IniConfigReaderWriter()
    //    {
    //        IConfigurationBuilder builder = new ConfigurationBuilder()
    //            .SetBasePath(Directory.GetCurrentDirectory())
    //            .AddIniFile(_iniPath, optional: true);

    //        _configuration = builder.Build();
    //    }

    //    public string GetValue(string section, string key, string defaultValue = null)
    //    {
    //        return $"{_configuration.GetSection(section).GetValue<string>(key)}";
    //    }

    //    public void SetValue(string section, string key, string value)
    //    {
    //        var obj = _configuration.GetSection(section);
    //        if (obj.Exists())
    //        {
    //            // 配置节存在
    //            foreach (IConfigurationSection child in obj.GetChildren())
    //            {
    //                if (child.Key.Trim() == $"{key}".Trim())
    //                {
    //                    obj.GetSection(child.Key).Value = $"{value}".Trim();
    //                    break;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            // 配置节不存在
    //            _configuration[$"{section}:{key}"] = $"{value}".Trim();
    //        }
    //    }

    //    public void Save()
    //    {
    //        using (var fs = new FileStream(_iniPath, FileMode.Create, FileAccess.Write))
    //        {
    //            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
    //            {
    //                foreach (var section in _configuration.GetChildren())
    //                {
    //                    sw.WriteLine($"[{section.Path}]");

    //                    foreach (var property in section.GetChildren())
    //                    {
    //                        sw.WriteLine($"{property.Key} = {property.Value}");
    //                    }

    //                    sw.WriteLine();
    //                }
    //            }
    //        }
    //    }
    //}
}
