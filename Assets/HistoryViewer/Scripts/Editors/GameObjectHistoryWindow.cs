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


        [MenuItem("Histories/GameObject")]
        private static void CreateWindow()
        {
            GetWindow<GameObjectHistoryWindow>();
        }

        public void OnGUI()
        {
            // ゲームオブジェクトの選択
            if (_target != Selection.activeTransform?.gameObject && Selection.activeTransform?.gameObject != null)
            {
                Debug.Log($"Selection: {_target} -> {Selection.activeObject}");
                _target = Selection.activeTransform?.gameObject;
            }

            if (_target != null)
            {
                EditorGUILayout.LabelField(_target.name);
            }
            else
            {
                foreach (var commit in _sceneGit.commits)
                {
                    EditorGUILayout.LabelField(commit.name);
                    foreach (var document in commit.unityYaml.documents)
                    {
                        if (document.IsGameObject)
                        {
                            EditorGUILayout.LabelField($"-- {document.gameObject.name}");
                        }
                    }
                }
                // EditorGUILayout.LabelField(_target.name);
            }
        }

        private void InitScene()
        {
            var logger = new UnityLogger();
            var git = new GitCommandExecutor(logger);

            _currentScene = SceneManager.GetActiveScene();
            _sceneGit = new SceneGit(git, _currentScene.path, logger);
            _sceneGit.LoadGitHistory();

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
