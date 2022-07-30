namespace Negi0109.HistoryViewer.Models
{
    public class AnyYaml
    {
        public readonly string name;

        public AnyYaml(UnityYamlDocument doc)
        {
            var name = doc.content.Split("\n", 3)[1];
            this.name = string.Intern(name[..^1]);
        }
    }
}
