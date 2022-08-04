namespace Negi0109.HistoryViewer.Models
{
    public class GitCommit
    {
        public bool IsLocalFile { get => hashId == null; }

        public readonly string hashId;
        public readonly string name;
        public UnityYaml unityYaml;

        public GitCommit(string hashId, string name)
        {
            this.hashId = hashId;
            this.name = name;
        }
    }
}
