using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using vNekoChatUI.Character.BingUtils.Enum;

namespace vNekoChatUI.Character.BingUtils
{
    //
    public class Bing_Response
    {
        [JsonPropertyName("totaltokens")]
        public int TotalTokens { get; set; } = int.MinValue;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        public string GetJsonStr()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });
        }
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
                    //"nlu_direct_response_filter",
                    //"deepleo",
                    //"disable_emoji_spoken_text",
                    //"responsible_ai_policy_235",
                    //"enablemm",
                    //"dv3sugg",
                    //"h3imaginative",
                    //"gencontentv3",

                    ////"travelansgnd",//-
                    ////"clgalileo",//-
                    ////"responseos",//-
                    ////"e2ecachewrite",//-
                    ////"cachewriteext",//-
                    ////"nodlcpcwrite",//-

                    //"cricketusertz", //+
                    //"recdxgnd", //+
                    ////"autosave", //+
                    //"iyxapbing", //+
                    //"iycapbing", //+

                    //20230805
                    "nlu_direct_response_filter",
                    "deepleo",
                    "disable_emoji_spoken_text",
                    "responsible_ai_policy_235",
                    "enablemm",
                    "dv3sugg",
                    "autosave",//
                    "iyxapbing",
                    "iycapbing",
                    "h3imaginative",
                    "gencontentv3",
                    "eredirecturl",

                    "nojbfedge",//no JailBreak Filter Edge
                    "nosearchall",
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
