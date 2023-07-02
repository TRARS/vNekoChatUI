using System;
using System.Collections.Generic;

namespace vNekoChatUI.Character.BingUtils.Models;

//old
//public class NetworkMessage
//{
//    public string text { get; set; }

//    public string messageType { get; set; } = "Chat";

//    public string inputMethod { get; set; } = "Keyboard";

//    public string author { get; set; } = "user";

//    public DateTimeOffset timestamp { get; set; }

//    public string region { get; set; } = "US";
//}

//new
public class NetworkMessage
{
    public string text { get; set; }

    public string messageType { get; set; } = "Chat";

    public string inputMethod { get; set; } = "Keyboard";

    public string author { get; set; } = "user";

    public DateTimeOffset timestamp { get; set; }

    //public string region { get; set; } = "JP";

    //private  string locale_str = "ja-JP";
    //"en-US"  "zh-cn"  "en-ie" "en-gb" "ja-JP"
    //"USA"    "CHINA"  "EU"    "UK"    "JP"
    public string locale { get; set; } //= locale_str;
    public string market { get; set; } //= locale_str;
    public string region { get; set; } //= locale_str.Substring(locale_str.Length - 2);
    public List<LocationHint> locationHints { get; set; } //= get_location_hint_from_locale(locale_str);

    public NetworkMessage(string _locale = null)
    {
        locale = market = _locale;
        region = _locale.Substring(_locale.Length - 2);
        locationHints = get_location_hint_from_locale(_locale);
    }
    private List<LocationHint> get_location_hint_from_locale(string locale)
    {
        switch (locale.ToLower())
        {
            case "en-us":
                {
                    return new()
                    {
                        new()
                        {
                                country = "United States",
                                state = "California",
                                city = "Los Angeles",
                                timezoneoffset = 8,
                                countryConfidence = 8,
                                Center = new()
                                {
                                    Latitude = 34.0536909,
                                    Longitude = -118.242766
                                },
                                RegionType = 2,
                                SourceType = 1
                        }
                    };
                }
            case "ja-jp":
                {
                    return new()
                    {
                        new()
                        {
                            country = "Japan",
                            state = "",
                            city = "Tokyo",
                            timezoneoffset = 9,
                            countryConfidence = 8,
                            Center = new()
                            {
                                Latitude = 35.6895,
                                Longitude = 139.6917
                            },
                            RegionType = 2,
                            SourceType = 1
                        }
                    };
                }
            case "zh-cn":
                {
                    return new()
                    {
                        new()
                        {
                                country = "China",
                                state = "",
                                city = "Beijing",
                                timezoneoffset = 8,
                                countryConfidence = 8,
                                Center = new()
                                {
                                    Latitude = 39.9042,
                                    Longitude = 116.4074
                                },
                                RegionType = 2,
                                SourceType = 1
                        }
                    };
                }
            case "en-gb":
                {
                    return new()
                    {
                        new()
                        {
                                country = "United Kingdom",
                                state = "",
                                city = "London",
                                timezoneoffset = 0,
                                countryConfidence = 8,
                                Center = new()
                                {
                                    Latitude = 51.5074,
                                    Longitude = -0.1278
                                },
                                RegionType = 2,
                                SourceType = 1
                        }
                    };
                }
            case "en-ie":
                {
                    return new()
                    {
                        new()
                        {
                                country = "Norway",
                                state = "",
                                city = "Oslo",
                                timezoneoffset = 1,
                                countryConfidence = 8,
                                Center = new()
                                {
                                    Latitude = 59.9139,
                                    Longitude = 10.7522
                                },
                                RegionType = 2,
                                SourceType = 1
                        }
                    };
                }
            default:
                {
                    return new()
                    {
                        new()
                        {
                                country = "United States",
                                state = "California",
                                city = "Los Angeles",
                                timezoneoffset = 8,
                                countryConfidence = 8,
                                Center = new()
                                {
                                    Latitude = 34.0536909,
                                    Longitude = -118.242766
                                },
                                RegionType = 2,
                                SourceType = 1
                        }
                    };
                }
        }
    }
}

public struct Locale
{
    public string USA = "en-US";
    public string CHINA = "zh-cn";
    public string EU = "en-ie";
    public string UK = "en-gb";
    public string JP = "ja-JP";

    public Locale() { }
}

public class LocationHint
{
    public class Center_
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public string country { get; set; }
    public string state { get; set; }
    public string city { get; set; }
    public int timezoneoffset { get; set; }
    public int countryConfidence { get; set; }
    public Center_ Center { get; set; }
    public int RegionType { get; set; }
    public int SourceType { get; set; }
}


public class ParticipantModel
{
    public string id { get; set; }
}

public class previousMessage
{
    public string author { get; set; }
    public string description { get; set; }
    public string contextType { get; set; }
    public string messageType { get; set; }
    public string messageId { get; set; }
}
