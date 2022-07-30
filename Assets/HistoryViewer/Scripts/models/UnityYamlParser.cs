using System.IO;
using System.Text;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYamlParser
    {
        private const string yamlDocumentDelimiter = "--- ";
        private readonly ILogger _logger;

        public UnityYamlParser(ILogger logger = null)
        {
            _logger = logger;
        }

        public UnityYaml Parse(StreamReader reader)
        {
            var yaml = new UnityYaml();
            var content = new StringBuilder();
            string name = null;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line.StartsWith(yamlDocumentDelimiter))
                {
                    yaml.documents.Add(new UnityYamlDocument(name, content.ToString()));
                    name = line[yamlDocumentDelimiter.Length..];
                    content = new StringBuilder();
                }

                content.Append(line + "\n");
            }

            yaml.documents.Add(new UnityYamlDocument(name, content.ToString()));

            return yaml;
        }
    }
}
