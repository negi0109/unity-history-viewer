using NUnit.Framework;
using Negi0109.HistoryViewer.Models;

namespace UnityYamlDocumentTest
{
    public class PrefabYamlTest
    {
        private static UnityYamlDocument.Factory _factory;

        [SetUp]
        public static void SetUp()
        {
            _factory = new UnityYamlDocument.Factory();
        }

        public static UnityYamlDocument GetDocument(string content)
        {
            return _factory.Get(content[4..content.IndexOf('\n')], content);
        }

        [TestCase(
            @"--- !u!1001 &2811843692065142958
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    m_TransformParent: {fileID: 0}
    m_Modifications:
    - target: {fileID: 4718096228747403906, guid: d746955d1e397469a9e57173ef97bef7, type: 3}
      propertyPath: m_Name
      value: Prefab1 Variant(stripped))
      objectReference: {fileID: 0}
    - target: {fileID: 4718096228747403967, guid: d746955d1e397469a9e57173ef97bef7, type: 3}
      propertyPath: m_RootOrder
      value: 0
      objectReference: {fileID: 0}", 4718096228747403906u
        )]
        public void ParseCorrespondingSourceObject(string content, ulong correspondingSourceObjectId)
        {
            var doc = GetDocument(content);
            var prefab = new PrefabYaml(doc);

            Assert.That(prefab.correspondingSourceObjectId, Is.EqualTo(correspondingSourceObjectId));
        }
    }
}
