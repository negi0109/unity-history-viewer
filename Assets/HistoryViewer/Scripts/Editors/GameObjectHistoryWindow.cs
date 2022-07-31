using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Negi0109.HistoryViewer.Models;
using Negi0109.HistoryViewer.Middleware;

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
            }

            try
            {
                // ゲームオブジェクトの選択
                if (_target != Selection.activeTransform?.gameObject)
                {
                    // Debug.Log($"Selection: {_target} -> {Selection.activeTransform?.gameObject?.name}");
                    _target = Selection.activeTransform?.gameObject;
                }

                if (_target != null)
                {
                    EditorGUILayout.LabelField(_target.name);

                    foreach (var commit in currentGit.commits)
                    {
                        EditorGUILayout.LabelField(commit.name);
                        if (
                            commit.unityYaml.TryGetGameObject(
                                isPrefabMode ?
                                GlobalObjectId.GetGlobalObjectIdSlow(_target).targetPrefabId
                                : GlobalObjectId.GetGlobalObjectIdSlow(_target).targetObjectId,
                                out var gameObjectYaml))
                        {
                            foreach (var componentId in gameObjectYaml.GameObject.componentIds)
                            {
                                if (commit.unityYaml.TryGetComponent(componentId, out var componentYaml))
                                {
                                    EditorGUILayout.LabelField($"-- {componentYaml.AnyObject.name}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var commit in currentGit.commits)
                    {
                        EditorGUILayout.LabelField(commit.name);
                        foreach (var document in commit.unityYaml.gameObjectDocuments.Values)
                        {
                            EditorGUILayout.LabelField($"-- {document.GameObject.name}");
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

                _currentScene = SceneManager.GetActiveScene();
                _sceneGit = new SceneGit(git, _currentScene.path, _logger);
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

                _currentPrefab = PrefabStageUtility.GetCurrentPrefabStage().assetPath;
                _prefabGit = new SceneGit(git, _currentPrefab, _logger);
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
