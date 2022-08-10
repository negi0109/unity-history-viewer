using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Negi0109.HistoryViewer.Models;
using Negi0109.HistoryViewer.Middleware;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Negi0109.HistoryViewer.Editors
{
    public class HistoryWindow : EditorWindow
    {
        private GameObject _target;
        private Scene _currentScene;
        private SceneGit _sceneGit;
        private SceneGit _prefabGit;
        private string _currentPrefab;

        private BufferedLogger _logger;

        private List<ObjectCommitDiff> diffs;
        private string _commitId = "";

        private VisualElement gameObjectHistory;

        [MenuItem("Histories/HistoryWindow")]
        public static void ShowExample()
        {
            HistoryWindow wnd = GetWindow<HistoryWindow>();
            wnd.titleContent = new GUIContent("HistoryWindow");
        }

        public void OnEnable()
        {
            var root = this.rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/HistoryViewer/Scripts/Editors/HistoryWindow.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/HistoryViewer/Scripts/Editors/HistoryWindow.uss");

            visualTree.CloneTree(root);
            root.styleSheets.Add(styleSheet);
            gameObjectHistory = root.Q("object-element");
        }

        // public void CreateGUI()
        // {
        //     // Import UXML
        //     var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/HistoryViewer/Scripts/Editors/HistoryWindow.uxml");
        //     VisualElement labelFromUXML = visualTree.Instantiate();
        //     root.Add(labelFromUXML);

        //     // A stylesheet can be added to a VisualElement.
        //     // The style will be applied to the VisualElement and all of its children.
        //     var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/HistoryViewer/Scripts/Editors/HistoryWindow.uss");
        //     VisualElement labelWithStyle = new Label("Hello World! With Style");
        //     labelWithStyle.styleSheets.Add(styleSheet);
        //     root.Add(labelWithStyle);
        // }

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
            }
            else
            {
                _currentPrefab = null;
                var currentScene = SceneManager.GetActiveScene();
                if (!currentScene.Equals(_currentScene)) InitScene();
            }

            if (_target != Selection.activeTransform?.gameObject)
            {
                Debug.Log($"Selection: {_target} -> {Selection.activeTransform?.gameObject?.name}");
                _target = Selection.activeTransform?.gameObject;

                if (_target != null)
                {
                    SerializedObject so = new(_target);
                    rootVisualElement.Bind(so);
                    gameObjectHistory.visible = true;
                }
                else
                {
                    gameObjectHistory.visible = false;
                }
            }

        }
    }
}
