namespace Negi0109.HistoryViewer.Models
{
    public static class YamlUtils
    {
        private const char HashKeyValuePairDelimiter = ':';
        private const char HashDelimiter = ',';
        private const string yamlDocumentDelimiter = "--- ";
        private const char SequenceEntry = '-';
        private const char IndentChar = ' ';

        public static string GetInlineKey(string text)
        {
            var delimiter = text.IndexOf(HashKeyValuePairDelimiter);
            if (delimiter == -1) return null;

            var start = text.LastIndexOf(IndentChar, delimiter);
            return text[(start + 1)..delimiter];
        }

        public static string GetInlineKey(string text, int startIndex)
        {
            var delimiter = text.IndexOf(HashKeyValuePairDelimiter, startIndex);
            if (delimiter == -1) return null;

            return text[startIndex..delimiter];
        }

        public static string GetInlineValue(string text, string key, int startIndex = 0)
        {
            return text[(text.IndexOf(HashKeyValuePairDelimiter, startIndex) + 2)..];
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

        public static bool IsKey(string text, string key)
        {
            var len = key.Length;
            var start = text.IndexOf(HashKeyValuePairDelimiter) - len;

            if (start < 0) return false;

            for (var i = 0; i < len; i++)
            {
                if (text[start + i] != key[i]) return false;
            }

            return true;
        }

        public static bool IsKey(string text, string key, int startIndex)
        {
            for (var i = 0; i < key.Length; i++)
            {
                if (text[startIndex + i] != key[i]) return false;
            }

            return text[startIndex + key.Length] == HashKeyValuePairDelimiter;
        }

        public static bool IsArrayElement(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == IndentChar) continue;
                return text[i] == SequenceEntry;
            }

            return false;
        }

        public static bool IsArrayElement(string text, int startIndex)
        {
            return text[startIndex] == SequenceEntry;
        }

        public static int GetIndentSize(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == IndentChar) continue;
                else return i;
            }

            return text.Length;
        }
    }
}
