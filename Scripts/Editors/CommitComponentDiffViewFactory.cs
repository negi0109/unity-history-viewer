using System.IO;
using Negi0109.HistoryViewer.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Negi0109.HistoryViewer.Editors
{
    using ComponentState = ObjectCommitDiff.CommitDiff.ComponentState;

    public class CommitComponentDiffViewFactory
    {
        private VisualTreeAsset _asset;

        public CommitComponentDiffViewFactory(VisualTreeAsset asset)
        {
            _asset = asset;
        }

        public VisualElement Build(ObjectCommitDiff.CommitDiff.Component component)
        {
            VisualElement child = new();
            _asset.CloneTree(child);
            child.AddToClassList(component.state.ToString());

            var stateLabel = child.Q<Label>("state");

            stateLabel.text = component.state switch
            {
                ComponentState.Add => "+",
                ComponentState.Destroy => "-",
                ComponentState.Change => "-+",
                _ => component.state.ToString()
            };

            var label = child.Q<Label>("name");
            var anyObject = component.state switch
            {
                ComponentState.Add => component.dest.AnyObject,
                ComponentState.Destroy => component.src.AnyObject,
                ComponentState.Change => component.src.AnyObject,
                _ => component.src.AnyObject
            };

            string anyObjectName = null;
            if (anyObject.guid != null)
            {
                var file = AssetDatabase.GUIDToAssetPath(anyObject.guid);
                if (!string.IsNullOrEmpty(file))
                {
                    anyObjectName = Path.GetFileNameWithoutExtension(file);
                }
            }
            anyObjectName ??= anyObject.name;
            label.text = anyObjectName;

            return child;
        }
    }
}
