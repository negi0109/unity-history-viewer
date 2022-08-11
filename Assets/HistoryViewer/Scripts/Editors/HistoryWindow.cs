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
        private string UXMLDirectory = "Assets/HistoryViewer/Scripts/Editors/UIElements/";
        // UIElementsアセット類
        private VisualTreeAsset rootAsset;
        private CommitDiffViewFactory _commitDiffFactory;

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
            _commitDiffFactory = new(
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXMLDirectory + "CommitDiff.uxml"),
                new(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXMLDirectory + "ComponentDiff.uxml"))
            );

            rootAsset.CloneTree(root);
            gameObjectHistory = root.Q("object-element");
            commitsView = gameObjectHistory.Q("commits");
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

                var child = _commitDiffFactory.Build(diff);
                if (i % 2 != 0) child.AddToClassList("alternate");

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
