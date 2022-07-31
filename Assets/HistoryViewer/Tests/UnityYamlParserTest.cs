using NUnit.Framework;
using Negi0109.HistoryViewer.Models;
using System.IO;
using System.Text;

public class UnityYamlParserTest
{
    public StreamReader _reader;
    public UnityYaml _unityYaml;

    [SetUp]
    public void SetUp()
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
        _reader = new StreamReader(stream);
        var parser = new UnityYamlParser();

        _unityYaml = parser.Parse(_reader);
    }

    [TearDown]
    public void TearDown()
    {
        _reader.Close();
    }

    [Test]
    public void ParseHeader()
    {
        Assert.That(_unityYaml.header.name, Is.Null);
        Assert.That(_unityYaml.header.content, Is.EqualTo(@"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
"));
    }

    [Test]
    public void ParseGameObjects()
    {
        Assert.That(_unityYaml.gameObjectDocuments[534047197].name, Is.EqualTo(@"!u!1 &534047197"));
        Assert.That(_unityYaml.gameObjectDocuments[534047197].content, Is.EqualTo(@"--- !u!1 &534047197
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
    }

    [Test]
    public void ParseAnyObjects()
    {
        Assert.That(_unityYaml.anyObjectDocumentDictionary[1].name, Is.EqualTo(@"!u!29 &1"));
        Assert.That(_unityYaml.anyObjectDocumentDictionary[1].content, Is.EqualTo(
          @"--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
"));
    }
}
