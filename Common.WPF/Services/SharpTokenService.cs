using SharpToken;
using System.Linq;

namespace Common.WPF.Services
{
    public interface ISharpTokenService
    {
        public (int, string, string) tiktoken_consumption(string input_str, string? model_str = null);
    }

    public partial class SharpTokenService : ISharpTokenService
    {
        const string encodingName = "cl100k_base";//p50k_edit //p50k_base //r50k_base

        /// <summary>
        /// <para>input: input_str, model_name</para>
        /// <para>return: (tiktoken_list_count, flag false if error)</para>
        /// </summary>
        /// <returns></returns>
        public (int, string, string) tiktoken_consumption(string input_str, string? model_str = null)
        {
            if (input_str.Trim().Length == 0) { return (0, "", ""); }

            model_str ??= encodingName;

            var encoding = GptEncoding.GetEncoding(model_str);
            var encoded = encoding.Encode(input_str); // Input:"Hello, world!"       Output: [9906, 11, 1917, 0]
            var decoded = encoding.Decode(encoded);   // Input: [9906, 11, 1917, 0]  Output: "Hello, world!"
            //计算结果跟网页版不一样

            var flag = input_str.Equals(decoded);
            var tokenList = string.Join(",", encoded);

            return (encoded.Count(), model_str, tokenList);
        }
    }
}
