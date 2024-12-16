
namespace vNekoChatUI.Character.GeminiUtils
{
    public class HttpClientWapper
    {
        GeminiApiWrapper _geminiApiWrapper = new();

        public async Task<string> Entry(string input, Func<CancellationToken> getCancellationToken)
        {
            try
            {
                var cancellationToken = getCancellationToken.Invoke();
                return await _geminiApiWrapper.UserSay(input, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"GeminiWrapper Error {ex.Message}");
            }
        }
    }

}
