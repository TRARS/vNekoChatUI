
namespace vNekoChatUI.Character.GeminiUtils
{
    public class HttpClientWapper
    {
        GeminiApiWrapper _geminiApiWrapper = new();

        public async Task<string> Entry(string input, Func<CancellationToken> getCancellationToken, Action<int> stepUp)
        {
            try
            {
                var cancellationToken = getCancellationToken.Invoke();
                return await _geminiApiWrapper.UserSay(input, cancellationToken, stepUp);
            }
            catch (Exception ex)
            {
                throw new Exception($"GeminiWrapper GeminiError {ex.Message}");
            }
        }
    }

}
