namespace Negi0109.HistoryViewer.Models
{
    public struct GitCommit
    {
        public readonly string hashId;
        public readonly string name;

        public GitCommit(string hashId, string name)
        {
            this.hashId = hashId;
            this.name = name;
        }
    }
}
