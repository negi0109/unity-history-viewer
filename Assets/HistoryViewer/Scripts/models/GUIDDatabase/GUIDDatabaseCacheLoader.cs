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
                content.AppendLine(kvp.Key + "," + kvp.Value);
            }
            _editorCache.Put(cacheLabel, key, content.ToString());
        }
    }
}
