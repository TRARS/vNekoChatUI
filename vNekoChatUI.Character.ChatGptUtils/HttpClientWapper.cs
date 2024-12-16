using System;
using System.Threading.Tasks;

namespace vNekoChatUI.Character.ChatGptUtils
{
    public partial class HttpClientWapper
    {
        ChatGptApiWrapper _chatGptApiWrapper = new();

        public async Task<string> Entry(string input)
        {
            try
            {
                return await _chatGptApiWrapper.UserSay(input);
            }
            catch (Exception ex)
            {
                throw new Exception($"ChatGptApiWrapper Error {ex.Message}");
            }
        }
    }
}
