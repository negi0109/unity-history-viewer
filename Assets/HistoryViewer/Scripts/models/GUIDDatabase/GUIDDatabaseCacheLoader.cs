using System.Text;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class GUIDDatabaseCacheLoader
    {
        public const string cacheLabel = "guid";
        IEditorCache _editorCache;
        string _label;
        ILogger _logger;

        public GUIDDatabaseCacheLoader(IEditorCache editorCache, string label = null, ILogger logger = null)
        {
            _editorCache = editorCache;
            _logger = logger;
            _label = cacheLabel;

            if (label is not null) _label = $"{cacheLabel}-{label}";
        }

        public bool Exists(string key)
        {
            var exists = _editorCache.Exists(_label, key);
            _logger?.Log($"{_label}: cache-{(exists ? "hit" : "miss")} {key}");

            return exists;
        }

        public void Put(string key, GUIDDatabase database)
        {
            _logger?.Log($"{_label}: put-cache {key}");

            var content = new StringBuilder();

            foreach (var kvp in database.dic)
            {
                content.Append(kvp.Key + "," + kvp.Value).Append('\n');
            }
            _editorCache.Put(_label, key, content.ToString());
        }

        public GUIDDatabase Get(string key)
        {
            _logger?.Log($"{_label}: get-cache {key}");

            GUIDDatabase db = new();
            _editorCache.Get(_label, key, reader =>
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var kvp = line.Split(",");

                    db.dic[kvp[0]] = kvp[1];
                }
            });

            return db;
        }
    }
}
