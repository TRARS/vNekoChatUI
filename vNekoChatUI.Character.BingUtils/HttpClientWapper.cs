using System;
using System.Threading.Tasks;

namespace vNekoChatUI.Character.BingUtils
{
    public partial class HttpClientWapper
    {
        BingGptApiWapper _bingChatApiWapper = new();

        public async Task<string> Entry(string input)
        {
            try
            {
                return await _bingChatApiWapper.UserSay(input);
            }
            catch (Exception ex)
            {
                throw new Exception($"BingChatApiWapper Error {ex.Message}");
            }
        }
    }
}
