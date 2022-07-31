using NUnit.Framework;
using Negi0109.HistoryViewer.Models;

namespace UnityYamlDocumentTest
{
    public class AnyYamlTest
    {
        private UnityYamlDocument.Factory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new UnityYamlDocument.Factory();
        }

        [TestCase("!u!81 &519420029",
    @"--- !u!81 &519420029
AudioListener:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 519420028}
  m_Enabled: 1
", "AudioListener")]
        public void ParseName(string name, string content, string objectName)
        {
            var doc = _factory.Get(name, content);
            var yaml = new AnyYaml(doc);

            Assert.That(yaml.name, Is.EqualTo(objectName));
        }
    }
}
