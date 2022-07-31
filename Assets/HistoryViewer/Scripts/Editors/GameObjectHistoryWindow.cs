using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Negi0109.HistoryViewer.Models;
using Negi0109.HistoryViewer.Middleware;

namespace Negi0109.HistoryViewer.Editors
{
    public class GameObjectHistoryWindow : EditorWindow
    {
        private GameObject _target;
        private Scene _currentScene;
        private SceneGit _sceneGit;
        private BufferedLogger _logger;


        [MenuItem("Histories/GameObject")]
        private static void CreateWindow()
        {
            GetWindow<GameObjectHistoryWindow>();
        }

        public void OnGUI()
        {
            try
            {
                // ゲームオブジェクトの選択
                if (_target != Selection.activeTransform?.gameObject)
                {
                    // Debug.Log($"Selection: {_target} -> {Selection.activeObject}");
                    _target = Selection.activeTransform?.gameObject;
                }

                if (_target != null)
                {
                    EditorGUILayout.LabelField(_target.name);

                    foreach (var commit in _sceneGit.commits)
                    {
                        EditorGUILayout.LabelField(commit.name);
                        if (commit.unityYaml.TryGetGameObject((int)GlobalObjectId.GetGlobalObjectIdSlow(_target).targetObjectId, out var gameObjectYaml))
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
                    foreach (var commit in _sceneGit.commits)
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
                _logger = new BufferedLogger(new UnityLogger(), true);

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

        private void Update()
        {
            var currentScene = SceneManager.GetActiveScene();

            if (!currentScene.Equals(_currentScene)) InitScene();
            if (_target != Selection.activeTransform?.gameObject) Repaint();
        }
    }
}
