using NUnit.Framework;
using Negi0109.HistoryViewer.Models;
using System.IO;
using System.Text;

public class UnityYamlParserTest
{
    [Test]
    public void Parse()
    {
        var yaml =
@"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
--- !u!1 &534047197
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 534047200}
  - component: {fileID: 534047199}
  - component: {fileID: 534047198}
  m_Layer: 0";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));
        var reader = new StreamReader(stream);
        var parser = new UnityYamlParser();

        var unityYaml = parser.Parse(reader);
        var documents = unityYaml.documents;

        Assert.That(unityYaml.documents[0].name, Is.Null);
        Assert.That(unityYaml.documents[0].content, Is.EqualTo(@"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
"));
        Assert.That(unityYaml.documents[1].name, Is.EqualTo(@"!u!29 &1"));
        Assert.That(unityYaml.documents[1].content, Is.EqualTo(@"--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
"));
        Assert.That(unityYaml.documents[2].name, Is.EqualTo(@"!u!1 &534047197"));
        Assert.That(unityYaml.documents[2].content, Is.EqualTo(@"--- !u!1 &534047197
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 534047200}
  - component: {fileID: 534047199}
  - component: {fileID: 534047198}
  m_Layer: 0
"));
        reader.Close();
    }
}
