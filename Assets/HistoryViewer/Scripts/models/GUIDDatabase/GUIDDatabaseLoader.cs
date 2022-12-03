using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class GUIDDatabaseLoader
    {
        IEditorCache _editorCache;
        IGitCommandExecutor _gitCommandExecutor;
        GUIDDatabaseCacheLoader _databaseCacheLoader;
        ILogger _logger;

        public GUIDDatabaseLoader(IEditorCache editorCache, IGitCommandExecutor gitCommandExecutor, ILogger logger = null)
        {
            _editorCache = editorCache;
            _gitCommandExecutor = gitCommandExecutor;
            _databaseCacheLoader = new GUIDDatabaseCacheLoader(_editorCache);
            _logger = logger;
        }

        public GUIDDatabase LoadGitGUIDDatabase(string hash)
        {
            if (_databaseCacheLoader.Exists(hash))
            {
                _logger?.Log("guidDatabase: cache-hit");
                return _databaseCacheLoader.Get(hash);
            }
            else
            {
                _logger?.Log("guidDatabase: cache-miss");

                var database = new GUIDDatabase();
                var files = _gitCommandExecutor.ExecGitCommand($"ls-tree -r --name-only {hash}").Split("\n", System.StringSplitOptions.RemoveEmptyEntries);

                foreach (var filename in files)
                {
                    if (!filename.EndsWith(".cs.meta") && !filename.EndsWith(".prefab.meta")) continue;

                    _gitCommandExecutor.ExecGitCommand($"show {hash}:\"{filename}\"", reader =>
                    {
                        var guid = GUIDUtils.GetFileGUID(reader);
                        if (guid is not null) database.dic.Add(guid, filename);
                    });
                }
                _logger?.Log("guidDatabase: put-cache");
                _databaseCacheLoader.Put(hash, database);

                return database;
            }
        }
    }
}
