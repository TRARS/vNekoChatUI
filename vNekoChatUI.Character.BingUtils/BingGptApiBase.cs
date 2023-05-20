using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using vNekoChatUI.Character.BingUtils.Enum;

namespace vNekoChatUI.Character.BingUtils
{
    //直接和ChatGptApiBase的一样
    public class InputData
    {
        [JsonPropertyName("ai_name")]
        public string Ai_Name { get; set; } = string.Empty;

        [JsonPropertyName("ai_profile")]
        public string Ai_Profile { get; set; } = string.Empty;

        [JsonPropertyName("ai_content")]
        public List<Ai_Content> Ai_Content { get; set; } = new();

        [JsonPropertyName("history_changed")]
        public bool History_Changed { get; set; } = false;
    }

    public class Ai_Content
    {
        [JsonPropertyName("roles")]
        public string Roles { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}

//ChatHelper
namespace vNekoChatUI.Character.BingUtils
{
    public static class ChatHelper
    {
        public static List<string> GetDefaultOptions(CharStyle style)
        {
            return style switch
            {
                CharStyle.Creative => new List<string>()
                {
                    "nlu_direct_response_filter",
                    "deepleo",
                    "disable_emoji_spoken_text",
                    "responsible_ai_policy_235",
                    "enablemm",
                    "h3imaginative",
                    "travelansgnd",
                    "dv3sugg",
                    "clgalileo",
                    "gencontentv3",
                    "dv3sugg",
                    "responseos",
                    "e2ecachewrite",
                    "cachewriteext",
                    "nodlcpcwrite",
                    "travelansgnd",
                    "nojbfedge",
                },
                CharStyle.Balanced => new List<string>()
                {
                    "nlu_direct_response_filter",
                    "deepleo",
                    "disable_emoji_spoken_text",
                    "responsible_ai_policy_235",
                    "enablemm",
                    "galileo",
                    "dv3sugg",
                    "responseos",
                    "e2ecachewrite",
                    "cachewriteext",
                    "nodlcpcwrite",
                    "travelansgnd",
                    "nojbfedge",
                },
                CharStyle.Precise => new List<string>()
                {
                    "nlu_direct_response_filter",
                    "deepleo",
                    "disable_emoji_spoken_text",
                    "responsible_ai_policy_235",
                    "enablemm",
                    "galileo",
                    "dv3sugg",
                    "responseos",
                    "e2ecachewrite",
                    "cachewriteext",
                    "nodlcpcwrite",
                    "travelansgnd",
                    "h3precise",
                    "clgalileo",
                    "nojbfedge",
                },
                _ => throw new NotImplementedException()
            };
        }

        public static List<string> GetDefaultResponseType()
        {
            return new List<string>()
            {
                "Chat",
                "InternalSearchQuery",
                "InternalSearchResult",
                "Disengaged",
                "InternalLoaderMessage",
                "RenderCardRequest",
                "AdsQuery",
                "SemanticSerp",
                "GenerateContentQuery",
                "SearchQuery"
            };
        }

        public static List<string> GetDefaultSlids()
        {
            return new List<string>()
            {
                "anidtestcf",
                "321bic62ups0",
                "sydpayajax",
                "sydperfinput",
                "303hubcancls0",
                "316cache_sss0",
                "323glpromptv3",
                "316e2ecache"
            };
        }
    }
}
