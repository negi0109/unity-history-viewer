using UnityEditor;
using UnityEngine;
using System.Linq;
using Negi0109.HistoryViewer.Models;
using Negi0109.HistoryViewer.Interfaces;
using UnityEngine.UIElements;

namespace Negi0109.HistoryViewer.Editors
{
    using GameObjectState = ObjectCommitDiff.CommitDiff.GameObjectState;

    public class CommitDiffViewFactory
    {
        private VisualTreeAsset _asset;
        private CommitComponentDiffViewFactory _componentDiffViewFactory;
        private Interfaces.ILogger _logger;

        public CommitDiffViewFactory(VisualTreeAsset asset, CommitComponentDiffViewFactory componentDiffViewFactory, Interfaces.ILogger logger = null)
        {
            _asset = asset;
            _componentDiffViewFactory = componentDiffViewFactory;
            _logger = logger;
        }

        public void Replace(ObjectCommitDiff diff, VisualElement child)
        {
            var label = child.Q<Label>("commit_name");
            var stateLabel = child.Q<Label>("state");
            var components = child.Q("components");

            child.RemoveFromClassList("disabled");
            components.RemoveFromClassList("disabled");
            components.Clear();

            label.text = diff.dest.name;

            if (diff.IsSame) child.AddToClassList("disabled");
            else if (diff.Diff?.gameObject != null && diff.Diff.gameObject.state != GameObjectState.Change)
            {
                var state = diff.Diff.gameObject.state;
                stateLabel.text = state switch
                {
                    GameObjectState.Add => "Create",
                    GameObjectState.Destroy => "Destroy",
                    GameObjectState.GameObjectToPrefab => "to Prefab",
                    GameObjectState.PrefabToGameObject => "to GameObject",
                    _ => state.ToString()
                };
                stateLabel.AddToClassList(state.ToString());
            }
            else
            {
                stateLabel.AddToClassList("disabled");
            }

            if (diff.Diff.components.Any())
            {
                foreach (var component in diff.Diff.components)
                {
                    components.Add(_componentDiffViewFactory.Build(component, diff));
                }
            }
            else
            {
                components.AddToClassList("disabled");
            }
        }

        public VisualElement Build(ObjectCommitDiff diff)
        {
            VisualElement child = new();
            _asset.CloneTree(child);

            Replace(diff, child);

            return child;
        }
    }
}
