using System.Collections.Generic;
using NUnit.Framework;
using Negi0109.HistoryViewer.Models;
using System.IO;
using System.Text;

namespace UnityYamlDocumentTest
{
    public class AnyYamlTest
    {
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
        public void ParseName(string content, string name)
        {
            var doc = new UnityYamlDocument("", content);
            var yaml = new AnyYaml(doc);

            Assert.That(yaml.name, Is.EqualTo(name));
        }
    }
}
