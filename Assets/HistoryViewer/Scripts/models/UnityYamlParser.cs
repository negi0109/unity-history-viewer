using System.Collections.Generic;
using System.IO;
using System.Text;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYamlParser
    {
        private const string yamlDocumentDelimiter = "--- ";
        private readonly ILogger _logger;
        private readonly Dictionary<string, UnityYamlDocument> _pool;

        public UnityYamlParser(Dictionary<string, UnityYamlDocument> pool = null, ILogger logger = null)
        {
            _pool = pool;
            _logger = logger;
        }

        public UnityYaml Parse(StreamReader reader)
        {
            var factory = new UnityYamlDocument.Factory(_pool, _logger);
            var yaml = new UnityYaml();
            var content = new StringBuilder();
            string name = null;

            do
            {
                var line = reader.ReadLine();

                if (line.StartsWith(yamlDocumentDelimiter))
                {
                    yaml.AddYamlDocument(factory.Get(name, content.ToString()));
                    name = line[yamlDocumentDelimiter.Length..];
                    content = new StringBuilder();
                }

                content.Append(line + "\n");
            } while (!reader.EndOfStream);

            yaml.AddYamlDocument(factory.Get(name, content.ToString()));

            return yaml;
        }
    }
}
