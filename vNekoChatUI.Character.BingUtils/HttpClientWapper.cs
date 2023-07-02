using System;
using System.Threading;
using System.Threading.Tasks;

namespace vNekoChatUI.Character.BingUtils
{
    public partial class HttpClientWapper
    {
        BingGptApiWapper _bingChatApiWapper = new();

        public async Task<string> Entry(string input, Func<CancellationToken> getCancellationToken, Action<int> stepUp)
        {
            try
            {
                var cancellationToken = getCancellationToken.Invoke();
                return await _bingChatApiWapper.UserSay(input, cancellationToken, stepUp);
            }
            catch (Exception ex)
            {
                throw new Exception($"BingChatApiWapper Error {ex.Message}");
            }
        }
    }
}
