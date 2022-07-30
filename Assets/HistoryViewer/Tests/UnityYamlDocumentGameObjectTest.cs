using System.Collections.Generic;
using NUnit.Framework;
using Negi0109.HistoryViewer.Models;
using System.IO;
using System.Text;

public class UnityYamlDocumentGameObjectTest
{
    [TestCase(
@"--- !u!1 &490523428
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 490523429}
  - component: {fileID: 490523430}
  - component: {fileID: 490523431}
  m_Layer: 0
  m_Name: Sample Object1
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1", "Sample Object1")]
    public void ParseName(string content, string name)
    {
        var doc = new UnityYamlDocument("", content);
        var gameObject = new GameObjectYaml(doc);

        Assert.That(gameObject.name, Is.EqualTo(name));

    }

    [TestCase(
@"--- !u!1 &490523428
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 490523429}
  - component: {fileID: 490523430}
  - component: {fileID: 490523431}
  m_Layer: 0
  m_Name: Sample Object1
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1", new int[] { 490523429, 490523430, 490523431 })]
    public void ParseComponents(string content, int[] ids)
    {
        var doc = new UnityYamlDocument("", content);
        var gameObject = new GameObjectYaml(doc, new TestLogger());

        Assert.That(gameObject.componentIds.ToArray(), Is.EqualTo(ids));
    }
}
