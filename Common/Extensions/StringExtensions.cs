using System.Text.RegularExpressions;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static string InsertSpaceBetweenConsecutiveNewlines(this string input)
        {
            input = input.Replace("\r", string.Empty);//砍掉\r

            string pattern = @"\n\n";
            string replacement = "\n \n";
            string result = Regex.Replace(input, pattern, replacement);

            //int index = result.IndexOf("\n\n");
            //Debug.WriteLine($"\n单个:{input.Split('\n').Length - 1}, 连续:{index > 0}");

            return result;
        }

        public static string RemoveCarriageReturns(this string input)
        {
            string pattern = "\r";
            string replacement = "";
            return Regex.Replace(input, pattern, replacement);
        }

        public static string RemoveZeroWidthChars(this string input)
        {
            string pattern = "\u200B";
            string replacement = "";
            return Regex.Replace(input, pattern, replacement);
        }
    }
}
