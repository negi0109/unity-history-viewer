using System.Collections.Generic;
using Negi0109.HistoryViewer.Middleware;
using Negi0109.HistoryViewer.Models;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        private GameObject _target;
        private Scene _currentScene;
        private SceneGit _sceneGit;
        private SceneGit _prefabGit;
        private string _currentPrefab;

        private BufferedLogger _logger;

        private GameObjectHistoryView gameObjectHistory;
        private SceneHistoryView sceneHistory;
        private EditorCache _editorCache;

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
            _logger ??= new BufferedLogger(new UnityLogger(), true);

            EditorSceneManager.sceneSaved += OnSceneSaved;
            PrefabStage.prefabSaved += OnPrefabSaved;

            rootAsset.CloneTree(root);
            gameObjectHistory = new(root.Q("gameObject-logs"), this, commitDiffAsset, componentDiffAsset, _logger);
            sceneHistory = new(root.Q("scene-logs"), this, sceneCommitAsset);

            root.Q("toolbar-showlog").RegisterCallback<ClickEvent>(ClickToolbarShowLogButton);


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

            gameObjectHistory.SetDirty(IsDirty());
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

        private void ReDrawCurrentCommitDiff()
        {
            gameObjectHistory.ReDrawCurrentCommitDiff();
        }

        private void ClickToolbarShowLogButton(ClickEvent e)
        {
            isShowLogs = !isShowLogs;
            DrawHeader();
            gameObjectHistory.DrawCommitDiffs(isShowLogs);
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
            _editorCache ??= new EditorCache();

            var sceneGit = new SceneGit(
                git,
                fileLoader,
                path,
                _editorCache,
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

            sceneHistory.Display();

            _logger?.PrintLog("UnityHistoryViewer-log: DisplayScene");
        }

        private void DisplayGameObjectLog()
        {
            gameObjectHistory.SetEnabled(true);
            sceneHistory.SetEnabled(false);

            gameObjectHistory.SetGameObject(_target);
            gameObjectHistory.DrawCommitDiffs(isShowLogs);

            _logger?.PrintLog("UnityHistoryViewer-log: SelectGameObject");
        }

        public bool IsDirty() => IsPrefabMode() ?
                PrefabStageUtility.GetCurrentPrefabStage().scene.isDirty :
                SceneManager.GetActiveScene().isDirty;

        public SceneGit CurrentGit() => IsPrefabMode() ? _prefabGit : _sceneGit;
        public bool IsPrefabMode() => PrefabStageUtility.GetCurrentPrefabStage() != null;
    }
}
