using UnityEditor;
using UnityEngine;
using System.Linq;
using Negi0109.HistoryViewer.Models;
using Negi0109.HistoryViewer.Middleware;
using System.Collections.Generic;
using UnityEditor.UIElements;
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
            label.text = component.state switch
            {
                ComponentState.Add => component.dest.AnyObject.name,
                ComponentState.Destroy => component.src.AnyObject.name,
                ComponentState.Change => component.src.AnyObject.name,
                _ => component.src.AnyObject.name
            };

            return child;
        }
    }
}
