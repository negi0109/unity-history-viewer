using System.IO;
using Negi0109.HistoryViewer.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Negi0109.HistoryViewer.Middleware;
using Negi0109.HistoryViewer.Models;

namespace Negi0109.HistoryViewer.Editors
{
    using ComponentState = ObjectCommitDiff.CommitDiff.ComponentState;

    public class CommitComponentDiffViewFactory
    {
        private VisualTreeAsset _asset;
        private Interfaces.ILogger _logger;
        private EditorCache _editorCache = new EditorCache();
        private GitCommandExecutor _git = new();

        public CommitComponentDiffViewFactory(VisualTreeAsset asset, Interfaces.ILogger logger)
        {
            _asset = asset;
            _logger = logger;
        }

        public VisualElement Build(ObjectCommitDiff.CommitDiff.Component component, ObjectCommitDiff commitDiff)
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
                } else {
                    var fileCommit = component.state switch
                    {
                        ComponentState.Add => commitDiff.dest,
                        ComponentState.Destroy => commitDiff.src,
                        _ => commitDiff.src
                    };
                    if (!fileCommit.IsLocalFile) {
                        fileCommit.LoadCsharpDatabase(_editorCache, _git, _logger);
                        if (fileCommit.csDatabase.TryGetValue(anyObject.guid, out string name)) {
                            anyObjectName = name;
                        }
                    }
                }
            }
            anyObjectName ??= anyObject.name;
            label.text = anyObjectName;

            return child;
        }
    }
}
