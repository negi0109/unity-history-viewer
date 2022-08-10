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
    using GameObjectState = ObjectCommitDiff.CommitDiff.GameObjectState;

    public class HistoryWindow : EditorWindow
    {
        private string UXMLDirectory = "Assets/HistoryViewer/Scripts/Editors/UIElements/";
        // UIElementsアセット類
        private VisualTreeAsset rootAsset;
        private VisualTreeAsset commitDiffAsset;


        private GameObject _target;
        private Scene _currentScene;
        private SceneGit _sceneGit;
        private SceneGit _prefabGit;
        private string _currentPrefab;

        private BufferedLogger _logger;

        private List<ObjectCommitDiff> diffs;
        private string _commitId = "";

        private VisualElement gameObjectHistory;
        private VisualElement commitsView;


        [MenuItem("Histories/HistoryWindow")]
        public static void ShowExample()
        {
            HistoryWindow wnd = GetWindow<HistoryWindow>();
            wnd.titleContent = new GUIContent("HistoryWindow");
        }

        public void OnEnable()
        {
            var root = this.rootVisualElement;
            rootAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXMLDirectory + "HistoryWindow.uxml");
            commitDiffAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXMLDirectory + "CommitDiff.uxml");

            rootAsset.CloneTree(root);
            gameObjectHistory = root.Q("object-element");
            commitsView = gameObjectHistory.Q("commits");
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

        private void SelectGameObject()
        {
            var isPrefabMode = PrefabStageUtility.GetCurrentPrefabStage() != null;
            var currentGit = isPrefabMode ? _prefabGit : _sceneGit;

            SerializedObject so = new(_target);
            rootVisualElement.Bind(so);

            diffs = new();

            var targetId = GlobalObjectId.GetGlobalObjectIdSlow(_target).targetObjectId
                                ^ GlobalObjectId.GetGlobalObjectIdSlow(_target).targetPrefabId;

            commitsView.Clear();

            for (var i = 1; i < currentGit.commits.Count; i++)
            {
                var diff = new ObjectCommitDiff(targetId, currentGit.commits[i], currentGit.commits[i - 1]);
                diffs.Add(diff);
                if (diff.IsNotExist) continue;

                VisualElement child = new();
                commitDiffAsset.CloneTree(child);

                var label = child.Q<Label>("commit_name");
                var stateLabel = child.Q<Label>("state");

                label.text = diff.dest.name;

                if (i % 2 != 0) child.AddToClassList("alternate");

                if (diff.IsSame) child.AddToClassList("disabled");
                else if (diff.Diff?.gameObject != null)
                {
                    var state = diff.Diff.gameObject.state;
                    stateLabel.text = state switch
                    {
                        GameObjectState.Add => "Create",
                        GameObjectState.Destroy => "Destroy",
                        GameObjectState.Change => "Change",
                        GameObjectState.GameObjectToPrefab => "to Prefab",
                        GameObjectState.PrefabToGameObject => "to GameObject",
                        _ => state.ToString()
                    };
                    stateLabel.AddToClassList(state.ToString());
                }
                else
                {
                    stateLabel.text = GameObjectState.Change.ToString();
                    stateLabel.AddToClassList(GameObjectState.Change.ToString());
                }

                commitsView.Add(child);
            }
            _logger.PrintLog("UnityHistoryViewer-log: SelectGameObject");
        }


        private void Update()
        {
            var isPrefabMode = PrefabStageUtility.GetCurrentPrefabStage() != null;
            var currentGit = isPrefabMode ? _prefabGit : _sceneGit;

            if (currentGit == null)
            {
                if (isPrefabMode) InitPrefab();
                else InitScene();

                _target = null;
            }

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
                _logger.Log($"Selection: {_target} -> {Selection.activeTransform?.gameObject?.name}");
                _target = Selection.activeTransform?.gameObject;

                if (_target != null)
                {
                    SelectGameObject();
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
