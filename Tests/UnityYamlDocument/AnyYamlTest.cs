using NUnit.Framework;
using Negi0109.HistoryViewer.Models;

namespace UnityYamlDocumentTest
{
    public class AnyYamlTest
    {
        private static UnityYamlDocument.Factory _factory;

        public static UnityYamlDocument GetDocument(string content)
        {
            _factory ??= new UnityYamlDocument.Factory();
            return _factory.Get(content[4..content.IndexOf('\n')], content);
        }

        [TestCase(
    @"--- !u!81 &519420029
AudioListener:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 519420028}
  m_Enabled: 1
", "AudioListener")]
        public void ParseName(string content, string objectName)
        {
            var doc = GetDocument(content);
            var yaml = new AnyYaml(doc);

            Assert.That(yaml.name, Is.EqualTo(objectName));
        }

        [TestCase(
    @"--- !u!81 &519420029
AudioListener:
  m_ObjectHideFlags: 0
  m_GameObject: {fileID: 519420028}
", 519420028u, true)]
        [TestCase(
    @"--- !u!81 &519420029
AudioListener:
  m_ObjectHideFlags: 0
", null, false)]
        public void ParseGameObjectId(string content, ulong gameObjectId, bool isBelongsToGameObject)
        {
            var doc = GetDocument(content);
            var yaml = new AnyYaml(doc);

            Assert.That(yaml.gameObjectId, Is.EqualTo(gameObjectId));
            Assert.That(yaml.IsBelongsToGameObject, Is.EqualTo(isBelongsToGameObject));
        }

        [TestCase(
            "512127049ff684744bb127413d29180a", @"--- !u!114 &1823714743
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Script: {fileID: 11500000, guid: 512127049ff684744bb127413d29180a, type: 3}
  m_GameObject: {fileID: 1823714741}"
        )]
        public void ParseComponentGUID(string guid, string content)
        {
            var doc = GetDocument(content);
            var yaml = new AnyYaml(doc);

            Assert.That(yaml.guid, Is.EqualTo(guid));
        }
    }
}
