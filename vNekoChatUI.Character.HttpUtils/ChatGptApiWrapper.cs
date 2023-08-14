using Common.WebWpfCommon;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace vNekoChatUI.Character.HttpUtils
{
    public partial class ChatGptApiWrapper
    {
        List<ChatGPTMessage> _messages { get; set; } = new();

        Action<string> system_say => (x) => { _messages.Add(new ChatGPTMessage(Roles.System, x)); };
        Action<string> user_say => (x) => { _messages.Add(new ChatGPTMessage(Roles.User, x)); };
        Action<string> assistant_say => (x) => { _messages.Add(new ChatGPTMessage(Roles.Assistant, x)); };
    }

    public partial class ChatGptApiWrapper
    {
        //下层负责和ChatGpt通信的HttpClien对象
        ChatGptApiClient _client = ChatGptApiClient.Instance;

        /// <summary>
        /// 参数 inputs: 完整的数据上下文，需用JSON格式化
        /// <para>返回值: 根据设定、聊天记录，ChatGPT作出的文字接龙回复</para>
        /// </summary>
        public async Task<string> UserSay(string inputs)
        {
            //0. 列表初始化
            _messages.Clear();

            //1. 解析JSON（如果传入数据格式不对，在这里就寄了）
            var jsonObject = JsonSerializer.Deserialize<InputData>(inputs, new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });

            var ai_name = jsonObject!.Ai_Name;
            var ai_profile = jsonObject.Ai_Profile;
            var ai_content = jsonObject.Ai_Content;

            //2. 载入AI设定
            if (ai_profile is not null)
            {
                system_say(ai_profile);
                //LogProxy.Instance.Print($"system: {jsonObject.Ai_Profile}");
            }

            //3. 载入聊天记录
            if (ai_content is not null)
            {
                foreach (var x in ai_content)
                {
                    switch (x.Roles)
                    {
                        case "User":
                            user_say(x.Content); break;
                        case "Assistant":
                            assistant_say(x.Content); break;
                    }
                    //LogProxy.Instance.Print($"{x.Roles}: {x.Content}");
                }
            }

            //4. 等待回复
            return await _client.WaitReplyAsync(_messages);
        }
    }
}
