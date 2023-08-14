using System;
using System.Collections.Generic;

namespace vNekoChatUI.Character.BingUtils.RequestNew
{
    public struct Locale
    {
        public string USA = "en-US";
        public string CHINA = "zh-cn";
        public string EU = "en-ie";
        public string UK = "en-gb";
        public string JP = "ja-JP";

        public Locale() { }
    }

    public class previousMessage
    {
        public string author { get; set; }
        public string description { get; set; }
        public string contextType { get; set; }
        public string messageType { get; set; }
        public string messageId { get; set; }
    }
}

namespace vNekoChatUI.Character.BingUtils.RequestNew
{
    public class NetworkRequestRoot
    {
        public List<Argument> arguments { get; set; }
        public string invocationId { get; set; }
        public string target { get; set; }
        public int type { get; set; }
    }
}

namespace vNekoChatUI.Character.BingUtils.RequestNew
{
    public class Argument
    {
        public string source { get; set; }
        public List<string> optionsSets { get; set; }
        public List<string> allowedMessageTypes { get; set; }
        public List<string> sliceIds { get; set; }
        public string verbosity { get; set; }
        public string scenario { get; set; }
        public string traceId { get; set; }
        public bool isStartOfSession { get; set; }
        public string requestId { get; set; }
        public Message message { get; set; }
        public string tone { get; set; }
        public string conversationSignature { get; set; }
        public Participant participant { get; set; }
        public string spokenTextMode { get; set; }
        public string conversationId { get; set; }

        public List<previousMessage> previousMessages { get; set; }
    }

    public class Center
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class LocationHint
    {
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public int timezoneoffset { get; set; }
        public int countryConfidence { get; set; }
        public int cityConfidence { get; set; }
        public Center Center { get; set; }
        public int RegionType { get; set; }
        public int SourceType { get; set; }
    }

    public class Message
    {
        public string locale { get; set; }
        public string market { get; set; }
        public string region { get; set; }
        public string location { get; set; }
        public List<LocationHint> locationHints { get; set; }
        public string userIpAddress { get; set; }
        public DateTimeOffset timestamp { get; set; }
        public string author { get; set; }
        public string inputMethod { get; set; }
        public string text { get; set; }
        public string messageType { get; set; }
        public string requestId { get; set; }
        public string messageId { get; set; }
        public string? imageUrl { get; set; } //"https://www.bing.com/images/blob?bcid=r4EE8q3Oo-AFAA";

        public Message(string _locale = null)
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

    public class Participant
    {
        public string id { get; set; }
    }


}
