using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Negi0109.HistoryViewer.Models;
using Negi0109.HistoryViewer.Middleware;
using System.Collections.Generic;

namespace Negi0109.HistoryViewer.Editors
{
    public class GameObjectHistoryWindow : EditorWindow
    {
        private GameObject _target;
        private Scene _currentScene;
        private SceneGit _sceneGit;
        private SceneGit _prefabGit;
        private string _currentPrefab;

        private BufferedLogger _logger;

        private List<ObjectCommitDiff> diffs;


        [MenuItem("Histories/GameObject")]
        private static void CreateWindow()
        {
            GetWindow<GameObjectHistoryWindow>();
        }

        public void OnGUI()
        {
            var isPrefabMode = PrefabStageUtility.GetCurrentPrefabStage() != null;
            var currentGit = isPrefabMode ? _prefabGit : _sceneGit;

            if (currentGit == null)
            {
                if (isPrefabMode) InitPrefab();
                else InitScene();

                currentGit = isPrefabMode ? _prefabGit : _sceneGit;
                _target = null;
            }

            try
            {
                // ゲームオブジェクトの選択
                if (_target != Selection.activeTransform?.gameObject)
                {
                    // Debug.Log($"Selection: {_target} -> {Selection.activeTransform?.gameObject?.name}");
                    _target = Selection.activeTransform?.gameObject;
                    diffs = new();

                    var targetId = isPrefabMode ?
                                GlobalObjectId.GetGlobalObjectIdSlow(_target).targetPrefabId
                                : GlobalObjectId.GetGlobalObjectIdSlow(_target).targetObjectId;

                    for (var i = 1; i < currentGit.commits.Count; i++)
                    {
                        diffs.Add(new ObjectCommitDiff(targetId, currentGit.commits[i], currentGit.commits[i - 1]));
                    }
                }

                if (_target != null)
                {
                    EditorGUILayout.LabelField(_target.name);

                    foreach (var diff in diffs)
                    {
                        GUI.enabled = !diff.IsSame;
                        EditorGUILayout.LabelField(diff.dest.name);
                    }
                    GUI.enabled = true;
                }
                else
                {
                    foreach (var commit in currentGit.commits)
                    {
                        EditorGUILayout.LabelField(commit.name);
                        foreach (var document in commit.unityYaml.gameObjectDocuments.Values)
                        {
                            EditorGUILayout.LabelField($"-- {document.document.GameObject.name}");
                        }
                    }
                    // EditorGUILayout.LabelField(_target.name);
                }
            }
            finally
            {
                _logger.PrintLog("UnityHistoryViewer-log: Repaint");
            }
        }

        private void InitScene()
        {
            try
            {
                _logger?.PrintLog();
                _logger ??= new BufferedLogger(new UnityLogger(), true);

                var git = new GitCommandExecutor(_logger);
                var fileLoader = new FileLoader(_logger);

                _currentScene = SceneManager.GetActiveScene();
                _sceneGit = new SceneGit(git, fileLoader, _currentScene.path, _logger);
                _sceneGit.LoadGitHistory();
            }
            finally
            {
                _logger.PrintLog("UnityHistoryViewer-log: LoadGitHistory");
            }

            Repaint();
        }

        private void InitPrefab()
        {
            try
            {
                _logger?.PrintLog();
                _logger ??= new BufferedLogger(new UnityLogger(), true);

                var git = new GitCommandExecutor(_logger);
                var fileLoader = new FileLoader(_logger);

                _currentPrefab = PrefabStageUtility.GetCurrentPrefabStage().assetPath;
                _prefabGit = new SceneGit(git, fileLoader, _currentPrefab, _logger);
                _prefabGit.LoadGitHistory();
            }
            finally
            {
                _logger.PrintLog("UnityHistoryViewer-log: LoadGitHistory");
            }

            Repaint();
        }


        private void Update()
        {
            var isPrefabMode = PrefabStageUtility.GetCurrentPrefabStage() != null;

            if (isPrefabMode)
            {
                var currentPrefab = PrefabStageUtility.GetCurrentPrefabStage().assetPath;
                if (!currentPrefab.Equals(_currentPrefab)) InitPrefab();
                if (_target != Selection.activeTransform?.gameObject) Repaint();
            }
            else
            {
                _currentPrefab = null;
                var currentScene = SceneManager.GetActiveScene();

                if (!currentScene.Equals(_currentScene)) InitScene();
                if (_target != Selection.activeTransform?.gameObject) Repaint();
            }
        }
    }
}
