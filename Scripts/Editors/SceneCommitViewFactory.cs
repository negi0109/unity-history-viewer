using Negi0109.HistoryViewer.Models;
using UnityEngine.UIElements;

namespace Negi0109.HistoryViewer.Editors
{
    public class SceneCommitViewFactory
    {
        private VisualTreeAsset _asset;

        public SceneCommitViewFactory(VisualTreeAsset asset)
        {
            _asset = asset;
        }

        public VisualElement Build(GitCommit commit)
        {
            VisualElement child = new();
            _asset.CloneTree(child);
            child.Q<Label>("commit_name").text = commit.name;

            return child;
        }
    }
}
