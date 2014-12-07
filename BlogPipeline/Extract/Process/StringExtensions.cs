using System.Text.RegularExpressions;

namespace BlogPipeline.Extract.Process
{
    public static class StringExtensions
    {
        private static readonly Regex RemoveTagsRegex = new Regex("<[^>]*>",
           RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string RemoveTags(this string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? ""
                : RemoveTagsRegex.Replace(value, "");
        }

        private static readonly Regex RemoveWhitespaceRegex = new Regex(@"\s+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string RemoveWhitespace(this string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? ""
                : RemoveWhitespaceRegex.Replace(value, " ").Trim();
        }

        public static string RemoveMdCrap(this string value)
        {
            return value.Replace("##", "").Replace("#", "");;
        }
    }
}
