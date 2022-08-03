using NUnit.Framework;
using Negi0109.HistoryViewer.Models;

namespace UnityYamlDocumentTest
{
    public class StrippedGameObjectYamlTest
    {
        private UnityYamlDocument.Factory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new UnityYamlDocument.Factory();
        }

        [TestCase("!u!1 &7385806036741806636 stripped",
    @"--- !u!1 &7385806036741806636 stripped
GameObject:
  m_CorrespondingSourceObject: {fileID: 4718096228747403906, guid: d746955d1e397469a9e57173ef97bef7, type: 3}
  m_PrefabInstance: {fileID: 2811843692065142958}
  m_PrefabAsset: {fileID: 0}
", 2811843692065142958u)]
        public void ParsePrefabId(string name, string content, ulong prefabId)
        {
            var doc = _factory.Get(name, content);
            var yaml = new StrippedGameObjectYaml(doc);

            Assert.That(yaml.prefabId, Is.EqualTo(prefabId));
        }
    }
}
