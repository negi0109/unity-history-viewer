using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class GUIDDatabaseManager
    {
        Dictionary<string, GUIDDatabase> guidDatabases = new Dictionary<string, GUIDDatabase>();
        GUIDDatabaseLoader loader;

        public GUIDDatabaseManager(IEditorCache editorCache, IGitCommandExecutor gitCommandExecutor, ILogger logger = null)
        {
            loader = new GUIDDatabaseLoader(editorCache, gitCommandExecutor, logger);
        }

        public void LoadGitGUIDDatabase(string hash)
        {
            if (guidDatabases.ContainsKey(hash)) return;

            guidDatabases.Add(hash, loader.LoadGitGUIDDatabase(hash));
        }
    }
}
