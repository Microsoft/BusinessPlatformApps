namespace RedditCore
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxChars)
        {
            return string.IsNullOrEmpty(value) || value.Length <= maxChars
                ? value
                : value.Substring(0, maxChars - 3) + "...";
        }

        public static string TrimEndString(this string value, string toRemove)
        {
            if (!string.IsNullOrEmpty(value) && value.EndsWith(toRemove))
            {
                return value.Remove(value.Length - toRemove.Length);
            }
            else
            {
                return value;
            }
        }
    }
}
