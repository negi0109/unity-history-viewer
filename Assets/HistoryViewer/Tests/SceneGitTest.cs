using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;
using Negi0109.HistoryViewer.Models;
using NUnit.Framework;

public class SceneGitTest
{
    public class LoadGitHistory
    {
        public class Sample1
        {
            private Dictionary<string, string> _gitDic;
            private Dictionary<string, string> _fileDic;
            private IGitCommandExecutor _git;
            private GUIDDatabaseManager _guidDatabaseManager = new GUIDDatabaseManager(new EditorCacheStub(), new GitCommandExecutorStub());
            private IFileLoader _fileLoader;

            [SetUp]
            public void SetUP()
            {
                _gitDic = new()
                {
                    {
                        "log.* --pretty=\"%H%x09%s\".* -- \"path\"",
                        "a1\tcommit1\na2\tcommit2"
                    },
                    {
                    "show a1:\"path\"",
                    @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &490523428
GameObject:
  m_Name: Sample Object a1
  m_IsActive: 1
"
                    },
                    {
                        "show a2:\"path\"",
                        @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &490523428
GameObject:
  m_Name: Sample Object a2
  m_IsActive: 1
"
                    }
                };
                _fileDic = new()
                {
                    {
                        "path",
                        @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &490523428
GameObject:
  m_Name: Sample Object Current
  m_IsActive: 1
"
                    }
                };
                _git = new GitCommandExecutorStub(_gitDic);
                _fileLoader = new FileLoaderStub(_fileDic);
            }

            [Test]
            public void LoadLocal()
            {
                var _sceneGit = new SceneGit(_git, _fileLoader, "path", _guidDatabaseManager);
                _sceneGit.LoadGitHistory();

                Assert.That(_sceneGit.commits[0].IsLocalFile, Is.True);
                Assert.That(_sceneGit.commits[1].IsLocalFile, Is.False);
                Assert.That(_sceneGit.commits[2].IsLocalFile, Is.False);
            }

            [Test]
            public void LoadName()
            {
                var _sceneGit = new SceneGit(_git, _fileLoader, "path", _guidDatabaseManager);
                _sceneGit.LoadGitHistory();

                Assert.That(_sceneGit.commits[0].name, Is.EqualTo("Current"));
                Assert.That(_sceneGit.commits[1].name, Is.EqualTo("commit1"));
                Assert.That(_sceneGit.commits[2].name, Is.EqualTo("commit2"));
            }

            [Test]
            public void LoadGameObjectNameLogs()
            {
                var _sceneGit = new SceneGit(_git, _fileLoader, "path", _guidDatabaseManager);
                _sceneGit.LoadGitHistory();

                Assert.That(_sceneGit.commits[0].unityYaml.gameObjectDocuments[490523428u].document.GameObject.name, Is.EqualTo("Sample Object Current"));
                Assert.That(_sceneGit.commits[1].unityYaml.gameObjectDocuments[490523428u].document.GameObject.name, Is.EqualTo("Sample Object a1"));
                Assert.That(_sceneGit.commits[2].unityYaml.gameObjectDocuments[490523428u].document.GameObject.name, Is.EqualTo("Sample Object a2"));
            }
        }
    }
}
