using Negi0109.HistoryViewer.Interfaces;
using System.Text;

namespace Negi0109.HistoryViewer.Models
{
    public class GUIDDatabaseCacheLoader
    {
        public const string cacheLabel = "guid";
        IEditorCache _editorCache;

        public GUIDDatabaseCacheLoader(IEditorCache editorCache)
        {
            _editorCache = editorCache;
        }

        public bool Exists(string key) => _editorCache.Exists(cacheLabel, key);

        public void Put(string key, GUIDDatabase database)
        {
            var content = new StringBuilder();

            foreach (var kvp in database.dic)
            {
                content.Append(kvp.Key + "," + kvp.Value).Append('\n');
            }
            _editorCache.Put(cacheLabel, key, content.ToString());
        }

        public GUIDDatabase Get(string key)
        {
            GUIDDatabase db = new();
            _editorCache.Get(cacheLabel, key, reader =>
            {
                do
                {
                    var line = reader.ReadLine();
                    var kvp = line.Split(",");

                    db.dic[kvp[0]] = kvp[1];
                } while (!reader.EndOfStream);
            });

            return db;
        }
    }
}
