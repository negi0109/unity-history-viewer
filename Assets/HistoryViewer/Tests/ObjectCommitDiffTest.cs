using System.Collections.Generic;
using NUnit.Framework;
using Negi0109.HistoryViewer.Models;

public class ObjectCommitDiffTest
{
    private static UnityYamlDocument.Factory _factory;

    public static UnityYamlDocument GetDocument(string content)
    {
        _factory ??= new UnityYamlDocument.Factory();
        return _factory.Get(content[4..content.IndexOf('\n')], content);
    }

    public class GameObjectState
    {

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
        [TestCase(
            0u,
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
        ", true, TestName = "Nothing GameObject"
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

        [TestCase(
            1823714741u,
            @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 490523429}
        ",
            @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1823714741}",
            @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1823714741}", true, TestName = "Is Same GameObject"
        )]
        [TestCase(
            1823714741u,
            @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 490523429}
        ",
            @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_GameObject: {fileID: 1823714741}",
            @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 2}
  m_GameObject: {fileID: 1823714741}", false, TestName = "Is not Same Component"
        )]
        [TestCase(
            1823714741u,
            @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 490523429}
        ",
            @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_GameObject: {fileID: 1}",
            @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_GameObject: {fileID: 1823714741}", false, TestName = "New Component"
        )]
        [TestCase(
            1823714741u,
            @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 490523429}
        ",
            @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_GameObject: {fileID: 1823714741}",
            @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_GameObject: {fileID: 1}", false, TestName = "Destroy Component"
        )]
        public void ObjectDiffWithComponent(ulong targetId, string gameObject, string commit1component, string commit2component, bool IsSame)
        {
            GitCommit commit1 = new("1", "commit 1");
            commit1.unityYaml = new();
            commit1.unityYaml.AddYamlDocument(GetDocument(gameObject));
            commit1.unityYaml.AddYamlDocument(GetDocument(commit1component));
            commit1.unityYaml.DissolveAssociations();

            GitCommit commit2 = new("2", "commit 2");
            commit2.unityYaml = new();
            commit2.unityYaml.AddYamlDocument(GetDocument(gameObject));
            commit2.unityYaml.AddYamlDocument(GetDocument(commit2component));
            commit2.unityYaml.DissolveAssociations();

            var diff = new ObjectCommitDiff(targetId, commit1, commit2);
            Assert.That(diff.IsSame, Is.EqualTo(IsSame));
        }

        public class GetCommitDiff
        {
            [TestCase(
              1823714741u,
              @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
            @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object2",
              ObjectCommitDiff.CommitDiff.GameObjectState.Change, TestName = "Change"
            )]
            [TestCase(
              1823714741u,
              @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
            @"--- !u!1 &11
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
              ObjectCommitDiff.CommitDiff.GameObjectState.Destroy, TestName = "Destroy"
            )]
            [TestCase(
              1823714741u,
              @"--- !u!1 &11
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
            @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
              ObjectCommitDiff.CommitDiff.GameObjectState.Add, TestName = "Add"
            )]
            public void GetCommitGameObject(ulong targetId, string commit1gameObject, string commit2gameObject, ObjectCommitDiff.CommitDiff.GameObjectState diffState)
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
                Assert.That(diff.Diff.gameObject.state, Is.EqualTo(diffState));
            }
        }
    }

    public class ComponentState
    {
        [TestCase(
                1823714741u,
                @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
  null,
                @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
                @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1823714741}",
                ObjectCommitDiff.CommitDiff.ComponentState.Add, TestName = "Add"
        )]
        [TestCase(
                1823714741u,
                @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
                @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1823714741}",
                @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
  null,
                ObjectCommitDiff.CommitDiff.ComponentState.Destroy, TestName = "Destroy"
        )]
        [TestCase(
                1823714741u,
                @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
                @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 2}
  m_GameObject: {fileID: 1823714741}",
                @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
                @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 5, z: 0, w: 0}
  m_GameObject: {fileID: 1823714741}",
                ObjectCommitDiff.CommitDiff.ComponentState.Change, TestName = "Change"
        )]
        [TestCase(
                1823714741u,
                @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
                @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 0}
  m_GameObject: {fileID: 1823714741}",
                @"--- !u!1 &1823714741
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  serializedVersion: 6
  m_Name: Sample Object1",
                @"--- !u!4 &490523429
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_LocalRotation: {x: 0, y: 5, z: 0, w: 0}
  m_GameObject: {fileID: 1823714741}",
                null, TestName = "Same"
        )]
        public void GetComponentDiff(ulong targetId,
          string commit1gameObject, string commit1component,
          string commit2gameObject, string commit2component,
          ObjectCommitDiff.CommitDiff.ComponentState? diffState)
        {
            GitCommit commit1 = new("1", "commit 1") { unityYaml = new() };
            commit1.unityYaml.AddYamlDocument(GetDocument(commit1gameObject));
            if (commit1component != null) commit1.unityYaml.AddYamlDocument(GetDocument(commit1component));
            commit1.unityYaml.DissolveAssociations();

            GitCommit commit2 = new("2", "commit 2") { unityYaml = new() };
            commit2.unityYaml = new();
            commit2.unityYaml.AddYamlDocument(GetDocument(commit2gameObject));
            if (commit2component != null) commit2.unityYaml.AddYamlDocument(GetDocument(commit2component));
            commit2.unityYaml.DissolveAssociations();

            var diff = new ObjectCommitDiff(targetId, commit1, commit2);
            if (diffState != null) Assert.That(diff.Diff.components[0].state, Is.EqualTo(diffState));
            else Assert.That(diff.Diff.components.Count, Is.Zero);
        }
    }
}
