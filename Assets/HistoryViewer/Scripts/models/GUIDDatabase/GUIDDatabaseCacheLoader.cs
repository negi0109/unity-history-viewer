using System.Text;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class GUIDDatabaseCacheLoader
    {
        public const string cacheLabel = "guid";
        IEditorCache _editorCache;
        string _label;

        public GUIDDatabaseCacheLoader(IEditorCache editorCache, string label = null)
        {
            _editorCache = editorCache;
            _label = cacheLabel;
            if (label is not null) _label = $"{cacheLabel}-{label}";
        }

        public bool Exists(string key) => _editorCache.Exists(_label, key);

        public void Put(string key, GUIDDatabase database)
        {
            var content = new StringBuilder();

            foreach (var kvp in database.dic)
            {
                content.Append(kvp.Key + "," + kvp.Value).Append('\n');
            }
            _editorCache.Put(_label, key, content.ToString());
        }

        public GUIDDatabase Get(string key)
        {
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
