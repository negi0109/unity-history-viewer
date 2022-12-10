using System.IO;
using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class CsGUIDDatabaseLoader
    {
        IEditorCache _editorCache;
        IGitCommandExecutor _gitCommandExecutor;
        GUIDDatabaseCacheLoader _databaseCacheLoader;
        ILogger _logger;

        public CsGUIDDatabaseLoader(IEditorCache editorCache, IGitCommandExecutor gitCommandExecutor, ILogger logger = null)
        {
            _editorCache = editorCache;
            _gitCommandExecutor = gitCommandExecutor;
            _databaseCacheLoader = new GUIDDatabaseCacheLoader(_editorCache, "cs", logger);
            _logger = logger;
        }

        public GUIDDatabase LoadGUIDDatabase(string hash)
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
                        var filepath = reader.ReadLine();
                        if (string.IsNullOrEmpty(filepath)) return;
                        if (!filepath.EndsWith(".cs.meta")) continue;

                        _gitCommandExecutor.ExecGitCommand($"show {hash}:\"{filepath}\"", reader =>
                        {
                            var guid = GUIDUtils.GetFileGUID(reader);
                            if (guid is not null) {
                                var filename = Path.GetFileName(filepath);
                                database.dic.Add(guid, filename[..^".cs.meta".Length]);
                            }
                        });
                    }
                });

                _databaseCacheLoader.Put(hash, database);

                return database;
            }
        }
    }
}
