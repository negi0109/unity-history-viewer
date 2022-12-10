using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class PrefabGUIDDatabaseLoader
    {
        IEditorCache _editorCache;
        IGitCommandExecutor _gitCommandExecutor;
        GUIDDatabaseCacheLoader _databaseCacheLoader;
        ILogger _logger;

        public PrefabGUIDDatabaseLoader(IEditorCache editorCache, IGitCommandExecutor gitCommandExecutor, ILogger logger = null)
        {
            _editorCache = editorCache;
            _gitCommandExecutor = gitCommandExecutor;
            _databaseCacheLoader = new GUIDDatabaseCacheLoader(_editorCache, "prefab", logger);
            _logger = logger;
        }

        public GUIDDatabase LoadGitGUIDDatabase(string hash)
        {
            if (_databaseCacheLoader.Exists(hash))
            {
                return _databaseCacheLoader.Get(hash);
            }
            else
            {
                var database = new GUIDDatabase();
                _gitCommandExecutor.ExecGitCommand($"ls-tree -r --name-only {hash}", (reader) =>
                {
                    while (!reader.EndOfStream)
                    {
                        var filename = reader.ReadLine();
                        if (string.IsNullOrEmpty(filename)) return;
                        if (!filename.EndsWith(".prefab.meta")) continue;

                        _gitCommandExecutor.ExecGitCommand($"show {hash}:\"{filename}\"", reader =>
                        {
                            var guid = GUIDUtils.GetFileGUID(reader);
                            if (guid is not null) database.dic.Add(guid, filename);
                        });
                    }
                });

                _databaseCacheLoader.Put(hash, database);

                return database;
            }
        }
    }
}
