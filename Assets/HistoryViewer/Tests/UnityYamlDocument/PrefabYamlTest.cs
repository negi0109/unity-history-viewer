using NUnit.Framework;
using Negi0109.HistoryViewer.Models;

namespace UnityYamlDocumentTest
{
    public class PrefabYamlTest
    {
        private UnityYamlDocument.Factory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new UnityYamlDocument.Factory();
        }
    }
}
