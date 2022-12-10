using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class GitCommit
    {
        public bool IsLocalFile { get => hashId == null; }

        public readonly string hashId;
        public readonly string name;
        public UnityYaml unityYaml;
        public GUIDDatabase prefabDatabase;
        public GUIDDatabase csDatabase;

        public GitCommit(string hashId, string name)
        {
            this.hashId = hashId;
            this.name = name;
        }

        public void LoadCsharpDatabase(IEditorCache editorCache, IGitCommandExecutor git, Interfaces.ILogger logger) {
            if (csDatabase is not null) return;

            var loader = new CsGUIDDatabaseLoader(editorCache, git, logger);
            csDatabase = loader.LoadGUIDDatabase(hashId);
        }
    }
}
