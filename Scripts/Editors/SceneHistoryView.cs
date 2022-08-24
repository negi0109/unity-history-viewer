using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace Negi0109.HistoryViewer.Editors
{
    public class SceneHistoryView
    {
        private readonly HistoryWindow _wnd;
        private readonly VisualElement _history;
        private readonly SceneCommitViewFactory _sceneCommitFactory;

        public SceneHistoryView(VisualElement history, HistoryWindow wnd, VisualTreeAsset sceneCommitAsset)
        {
            _history = history;
            _wnd = wnd;
            _sceneCommitFactory = new SceneCommitViewFactory(sceneCommitAsset);
        }

        public void SetEnabled(bool b) => _history.SetEnabled(b);

        public void Display()
        {
            var scene = _wnd.CurrentGit();

            _history.Q<Label>("SceneName").text = _wnd.IsPrefabMode() ?
                PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.name :
                SceneManager.GetActiveScene().name;

            var commitsView = _history.Q("commits");
            commitsView.Clear();

            var commits = scene.commits;

            for (var i = 0; i < commits.Count; i++)
            {
                var commit = commits[i];

                var child = _sceneCommitFactory.Build(commit);
                if (i % 2 == 0) child.AddToClassList("alternate");

                commitsView.Add(child);
            }
        }
    }
}
