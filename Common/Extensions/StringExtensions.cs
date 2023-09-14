using System.Text.RegularExpressions;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 往连续的换行符之间插入一个空格
        /// </summary>
        public static string InsertSpaceBetweenConsecutiveNewlines(this string input)
        {
            input = input.Replace("\r", string.Empty);//砍掉\r

            string pattern = @"\n\n";
            string replacement = "\n \n";
            string result = Regex.Replace(input, pattern, replacement);

            return result;
        }

        /// <summary>
        /// 删除换行符之间的空白内容
        /// </summary>
        public static string RemoveSpaceBetweenConsecutiveNewlines(this string input)
        {
            string pattern = @"\n\s+\n"; // 匹配连续的换行符之间的一个或多个空格
            string replacement = "\n\n"; // 将匹配到的内容替换为连续的换行符
            string result = Regex.Replace(input, pattern, replacement);

            return result;
        }

        /// <summary>
        /// 删除\r
        /// </summary>
        public static string RemoveCarriageReturns(this string input)
        {
            string pattern = "\r";
            string replacement = "";
            return Regex.Replace(input, pattern, replacement);
        }

        /// <summary>
        /// 删除\u200B
        /// </summary>
        public static string RemoveZeroWidthChars(this string input)
        {
            string pattern = "\u200B";
            string replacement = "";
            return Regex.Replace(input, pattern, replacement);
        }
    }
}
