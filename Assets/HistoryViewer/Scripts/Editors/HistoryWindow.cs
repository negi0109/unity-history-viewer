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
        // UIElementsアセット類
        public VisualTreeAsset rootAsset;
        public VisualTreeAsset commitDiffAsset;
        public VisualTreeAsset sceneCommitAsset;
        public VisualTreeAsset componentDiffAsset;
        private CommitDiffViewFactory _commitDiffFactory;
        private SceneCommitViewFactory _sceneCommitFactory;

        private GameObject _target;
        private Scene _currentScene;
        private SceneGit _sceneGit;
        private SceneGit _prefabGit;
        private string _currentPrefab;

        private BufferedLogger _logger;

        private List<ObjectCommitDiff> diffs;

        private VisualElement gameObjectHistory;
        private VisualElement sceneHistory;
        private VisualElement dirtyFlagElement;

        private bool isShowLogs = false;

        [MenuItem("Histories/HistoryWindow")]
        public static void ShowWindow()
        {
            HistoryWindow wnd = GetWindow<HistoryWindow>();
            wnd.titleContent = new GUIContent("HistoryWindow");
        }

        public void OnEnable()
        {
            var root = rootVisualElement;
            _commitDiffFactory = new(
                commitDiffAsset,
                new CommitComponentDiffViewFactory(componentDiffAsset)
            );
            _sceneCommitFactory = new(sceneCommitAsset);

            EditorSceneManager.sceneSaved += OnSceneSaved;
            PrefabStage.prefabSaved += OnPrefabSaved;

            rootAsset.CloneTree(root);
            gameObjectHistory = root.Q("gameObject-logs");
            dirtyFlagElement = gameObjectHistory.Q("GameObjectDirty");
            sceneHistory = root.Q("scene-logs");
            root.Q("toolbar-showlog").RegisterCallback<ClickEvent>(ClickToolbarShowLogButton);

            _logger ??= new BufferedLogger(new UnityLogger(), true);

            DrawHeader();
        }

        private void Update()
        {
            if (TryLoadCurrentGit()) _target = null;

            if (_target != Selection.activeTransform?.gameObject)
            {
                // _logger?.Log($"Selection: {_target} -> {Selection.activeTransform?.gameObject?.name}");
                _target = Selection.activeTransform?.gameObject;

                if (_target != null)
                {
                    DisplayGameObjectLog();
                }
                else
                {
                    DisplaySceneLog();
                }
            }

            dirtyFlagElement.SetEnabled(IsDirty());
        }

        private void OnSceneSaved(Scene scene)
        {
            _logger?.Log($"saved {scene.name}");
            _sceneGit?.ReloadCurrentFile();
            ReDrawCurrentCommitDiff();

            _logger?.PrintLog("Save Scene");
        }

        private void OnPrefabSaved(GameObject go)
        {
            _logger?.Log($"saved prefab {go.name}");
            _sceneGit?.ReloadCurrentFile();
            ReDrawCurrentCommitDiff();

            _logger?.PrintLog("Save Prefab");
        }

        private void CacheCommitDiffs()
        {
            diffs = new();

            var targetGOId = GlobalObjectId.GetGlobalObjectIdSlow(_target);
            var targetId = targetGOId.targetObjectId ^ targetGOId.targetPrefabId;

            var commits = CurrentGit().commits;

            for (var i = 1; i < commits.Count; i++)
            {
                var diff = new ObjectCommitDiff(targetId, commits[i], commits[i - 1]);
                diffs.Add(diff);
            }
        }

        private void DrawCommitDiffs()
        {
            var commitsView = gameObjectHistory.Q("commits");
            commitsView.Clear();

            var alternate = true;
            for (var i = 0; i < diffs.Count; i++)
            {
                var diff = diffs[i];

                if (i != 1 && diff.IsSame && !isShowLogs) continue;

                var child = _commitDiffFactory.Build(diff);
                if (alternate) child.AddToClassList("alternate");

                commitsView.Add(child);

                alternate = !alternate;
            }
        }

        private void ReDrawCurrentCommitDiff()
        {
            var isPrefabMode = PrefabStageUtility.GetCurrentPrefabStage() != null;
            var currentGit = isPrefabMode ? _prefabGit : _sceneGit;

            var targetGOId = GlobalObjectId.GetGlobalObjectIdSlow(_target);
            var targetId = targetGOId.targetObjectId ^ targetGOId.targetPrefabId;
            var commitsView = gameObjectHistory.Q("commits");

            if (commitsView.childCount >= 1)
            {
                diffs[0] = new ObjectCommitDiff(targetId, currentGit.commits[1], currentGit.commits[0]);

                _commitDiffFactory.Replace(
                    diffs[0],
                    commitsView[0]
                );
            }
        }

        private void ClickToolbarShowLogButton(ClickEvent e)
        {
            isShowLogs = !isShowLogs;
            DrawHeader();
            DrawCommitDiffs();
        }

        private void DrawHeader()
        {
            var img = rootVisualElement.Q("toolbar-showlog").Q<Image>();
            img.image = EditorGUIUtility.IconContent(
                isShowLogs ? "d_VisibilityOn" : "d_VisibilityOff"
            ).image;
        }

        private SceneGit LoadSceneGit(string path)
        {
            var git = new GitCommandExecutor(_logger);
            var fileLoader = new FileLoader(_logger);

            var sceneGit = new SceneGit(
                git,
                fileLoader,
                path,
                _logger
            );
            sceneGit.LoadGitHistory();

            return sceneGit;
        }

        private void LoadSceneLog()
        {
            _currentScene = SceneManager.GetActiveScene();
            _sceneGit = LoadSceneGit(_currentScene.path);
        }

        private void LoadPrefabLog()
        {
            _currentPrefab = PrefabStageUtility.GetCurrentPrefabStage().assetPath;
            _prefabGit = LoadSceneGit(_currentPrefab);
        }

        private bool TryLoadCurrentGit()
        {
            if (IsPrefabMode())
            {
                var currentPrefab = PrefabStageUtility.GetCurrentPrefabStage().assetPath;
                if (_currentPrefab == null || !currentPrefab.Equals(_currentPrefab))
                {
                    LoadPrefabLog();
                    return true;
                }
            }
            else
            {
                var currentScene = SceneManager.GetActiveScene();
                if (_currentScene == null || !currentScene.Equals(_currentScene))
                {
                    LoadSceneLog();
                    return true;
                }
            }

            return false;
        }

        private void DisplaySceneLog()
        {
            gameObjectHistory.SetEnabled(false);
            sceneHistory.SetEnabled(true);


            if (TryLoadCurrentGit()) _target = null;

            var commits = CurrentGit().commits;

            sceneHistory.Q<Label>("SceneName").text = IsPrefabMode() ?
                PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.name :
                SceneManager.GetActiveScene().name;

            var commitsView = sceneHistory.Q("commits");
            commitsView.Clear();

            for (var i = 0; i < commits.Count; i++)
            {
                var commit = commits[i];

                var child = _sceneCommitFactory.Build(commit);
                if (i % 2 == 0) child.AddToClassList("alternate");

                commitsView.Add(child);
            }
            _logger?.PrintLog("UnityHistoryViewer-log: DisplayScene");
        }

        private void DisplayGameObjectLog()
        {
            gameObjectHistory.SetEnabled(true);
            sceneHistory.SetEnabled(false);

            SerializedObject so = new(_target);
            rootVisualElement.Bind(so);

            CacheCommitDiffs();
            DrawCommitDiffs();

            _logger?.PrintLog("UnityHistoryViewer-log: SelectGameObject");
        }

        private bool IsDirty() => IsPrefabMode() ?
                PrefabStageUtility.GetCurrentPrefabStage().scene.isDirty :
                SceneManager.GetActiveScene().isDirty;

        private SceneGit CurrentGit() => IsPrefabMode() ? _prefabGit : _sceneGit;
        private bool IsPrefabMode() => PrefabStageUtility.GetCurrentPrefabStage() != null;
    }
}
