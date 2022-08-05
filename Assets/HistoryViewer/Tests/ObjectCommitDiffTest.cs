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

    [TestCase(
        1823714741u,
        "!u!1 &1823714741",
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ",
        "!u!1 &1823714741",
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ", true, TestName = "Is Same GameObject"
    )]
    [TestCase(
        1823714741u,
        "!u!1 &1823714741",
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ",
        "!u!1 &1823714741",
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 7
        ", true, TestName = "Is not Same GameObject"
    )]
    [TestCase(
        1823714741u,
        "!u!1 &1823714741",
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ",
        "!u!1 &34",
        @"--- !u!1 &34
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ", false, TestName = "Destroy GameObject"
    )]
    [TestCase(
        1823714741u,
        "!u!1 &34",
        @"--- !u!1 &34
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ",
        "!u!1 &1823714741",
        @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
        ", false, TestName = "Created GameObject"
    )]
    public void ObjectDiff(ulong targetId, string commit1name, string commit1doc, string commit2name, string commit2doc, bool IsSame)
    {
        var commit1 = new GitCommit("1", "commit 1");
        commit1.unityYaml = new UnityYaml();
        commit1.unityYaml.AddYamlDocument(_factory.Get(commit1name, commit1doc));
        commit1.unityYaml.DissolveAssociations();

        var commit2 = new GitCommit("2", "commit 2");
        commit2.unityYaml = new UnityYaml();
        commit2.unityYaml.AddYamlDocument(_factory.Get(commit2name, commit2doc));
        commit2.unityYaml.DissolveAssociations();

        var diff = new ObjectCommitDiff(targetId, commit1, commit2);
        Assert.That(diff.IsSame, Is.EqualTo(IsSame));
    }
}
