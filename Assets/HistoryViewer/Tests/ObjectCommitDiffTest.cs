using System.Collections.Generic;
using NUnit.Framework;
using Negi0109.HistoryViewer.Models;

public class ObjectCommitDiffTest
{
    private UnityYamlDocument.Factory _factory;

    [SetUp]
    public void SetUp()
    {
        _factory = new UnityYamlDocument.Factory();
    }

    public UnityYamlDocument GetDocument(string content)
    {
        return _factory.Get(content[4..content.IndexOf('\n')], content);
    }

    [TestCase(
        1823714741u,
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ",
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ", true, TestName = "Is Same GameObject"
    )]
    [TestCase(
        1823714741u,
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ",
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 7
        ", false, TestName = "Is not Same GameObject"
    )]
    [TestCase(
        1823714741u,
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ",
        @"--- !u!1 &34
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ", false, TestName = "Destroy GameObject"
    )]
    [TestCase(
        1823714741u,
        @"--- !u!1 &34
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ",
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ", false, TestName = "Created GameObject"
    )]
    public void ObjectDiff(ulong targetId, string commit1gameObject, string commit2gameObject, bool IsSame)
    {
        GitCommit commit1 = new("1", "commit 1");
        commit1.unityYaml = new();
        commit1.unityYaml.AddYamlDocument(GetDocument(commit1gameObject));
        commit1.unityYaml.DissolveAssociations();

        GitCommit commit2 = new("2", "commit 2");
        commit2.unityYaml = new();
        commit2.unityYaml.AddYamlDocument(GetDocument(commit2gameObject));
        commit2.unityYaml.DissolveAssociations();

        var diff = new ObjectCommitDiff(targetId, commit1, commit2);
        Assert.That(diff.IsSame, Is.EqualTo(IsSame));
    }
}
