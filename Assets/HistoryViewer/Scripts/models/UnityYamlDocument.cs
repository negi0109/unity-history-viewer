using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYamlDocument
    {
        public readonly string name;
        public readonly string content;

        public UnityYamlDocument(string name, string content)
        {
            this.name = name;
            this.content = content;
        }
    }
}
