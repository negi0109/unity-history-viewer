using UnityEditor;
using UnityEngine;
using Negi0109.HistoryViewer.Models;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Negi0109.HistoryViewer.Editors
{
    public class GameObjectHistoryView
    {
        private GameObject _target;
        private List<ObjectCommitDiff> diffs;

        private readonly HistoryWindow _wnd;
        private readonly VisualElement _history;
        private readonly CommitDiffViewFactory _gameObjectCommitFactory;
        private readonly VisualElement dirtyFlagElement;

        public GameObjectHistoryView(VisualElement history, HistoryWindow wnd, VisualTreeAsset commitDiffAsset, VisualTreeAsset componentDiffAsset)
        {
            _wnd = wnd;
            _history = history;
            _gameObjectCommitFactory = new(commitDiffAsset, new CommitComponentDiffViewFactory(componentDiffAsset));

            dirtyFlagElement = _history.Q("GameObjectDirty");
        }

        public void SetGameObject(GameObject target)
        {
            _target = target;

            _history.Bind(new(_target));

            diffs = new();

            var targetGOId = GlobalObjectId.GetGlobalObjectIdSlow(target);
            var targetId = targetGOId.targetObjectId ^ targetGOId.targetPrefabId;

            var commits = _wnd.CurrentGit().commits;

            for (var i = 1; i < commits.Count; i++)
            {
                var diff = new ObjectCommitDiff(targetId, commits[i], commits[i - 1]);
                diffs.Add(diff);
            }

        }

        public void DrawCommitDiffs(bool isShowLogs)
        {
            var commitsView = _history.Q("commits");
            commitsView.Clear();

            var alternate = true;
            for (var i = 0; i < diffs.Count; i++)
            {
                var diff = diffs[i];

                if (i != 0 && diff.IsSame && !isShowLogs) continue;

                var child = _gameObjectCommitFactory.Build(diff);
                if (alternate) child.AddToClassList("alternate");

                commitsView.Add(child);

                alternate = !alternate;
            }
        }

        public void ReDrawCurrentCommitDiff()
        {
            var currentGit = _wnd.CurrentGit();

            var targetGOId = GlobalObjectId.GetGlobalObjectIdSlow(_target);
            var targetId = targetGOId.targetObjectId ^ targetGOId.targetPrefabId;
            var commitsView = _history.Q("commits");

            if (commitsView.childCount >= 1)
            {
                diffs[0] = new ObjectCommitDiff(targetId, currentGit.commits[1], currentGit.commits[0]);

                _gameObjectCommitFactory.Replace(
                    diffs[0],
                    commitsView[0]
                );
            }
        }

        public void SetEnabled(bool b) => _history.SetEnabled(b);

        public void SetDirty(bool v) => dirtyFlagElement.SetEnabled(v);
    }
}
