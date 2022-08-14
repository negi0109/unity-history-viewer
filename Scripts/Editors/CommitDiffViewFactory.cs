using UnityEditor;
using UnityEngine;
using System.Linq;
using Negi0109.HistoryViewer.Models;
using UnityEngine.UIElements;

namespace Negi0109.HistoryViewer.Editors
{
    using GameObjectState = ObjectCommitDiff.CommitDiff.GameObjectState;

    public class CommitDiffViewFactory
    {
        private VisualTreeAsset _asset;
        private CommitComponentDiffViewFactory _componentDiffViewFactory;

        public CommitDiffViewFactory(VisualTreeAsset asset, CommitComponentDiffViewFactory componentDiffViewFactory)
        {
            _asset = asset;
            _componentDiffViewFactory = componentDiffViewFactory;
        }

        public VisualElement Build(ObjectCommitDiff diff)
        {
            VisualElement child = new();
            _asset.CloneTree(child);


            var label = child.Q<Label>("commit_name");
            var stateLabel = child.Q<Label>("state");

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

            var components = child.Q("components");
            if (diff.Diff.components.Any())
            {
                foreach (var component in diff.Diff.components)
                {
                    components.Add(_componentDiffViewFactory.Build(component));
                }
            }
            else
            {
                components.AddToClassList("disabled");
            }

            return child;
        }
    }
}
