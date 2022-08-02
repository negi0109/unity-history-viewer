namespace Negi0109.HistoryViewer.Models
{
    public static class YamlUtils
    {
        private const string HashKeyValuePairDelimiter = ":";
        private const string HashDelimiter = ",";
        private const string yamlDocumentDelimiter = "--- ";

        public static string GetInlineValue(string text, string key)
        {
            return text[(text.IndexOf(HashKeyValuePairDelimiter) + 2)..];
        }

        public static string GetBlockValue(string text, string key)
        {
            // {a1: b1, a2: b2}
            var tmp1 = text[1..^1];
            // a1: b1, a2: b2
            var start = tmp1.IndexOf(key);
            var end = tmp1.IndexOf(HashDelimiter, start);

            if (end == -1)
            {
                return tmp1[(start + key.Length + 2)..];
            }
            else
            {
                return tmp1[(start + key.Length + 2)..end];
            }
        }

        public static bool IsDocumentDelimiter(string text)
        {
            return text.StartsWith(yamlDocumentDelimiter);
        }

        public static string GetDocumentName(string text)
        {
            return text[yamlDocumentDelimiter.Length..];
        }
    }
}
