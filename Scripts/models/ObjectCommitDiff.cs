using System.Collections.Generic;
using System.Linq;

namespace Negi0109.HistoryViewer.Models
{
    public class ObjectCommitDiff
    {
        public class CommitDiff
        {
            public enum GameObjectState
            {
                Add,
                Destroy,
                Change,
                GameObjectToPrefab,
                PrefabToGameObject,
            }

            public enum ComponentState
            {
                Add,
                Destroy,
                Change
            }

            public class GameObject
            {
                public GameObjectState state;
            }

            public class Component
            {
                public readonly ComponentState state;
                public readonly UnityYamlDocument src;
                public readonly UnityYamlDocument dest;

                public Component(ComponentState state, UnityYamlDocument src, UnityYamlDocument dest)
                {
                    this.state = state;
                    this.src = src;
                    this.dest = dest;
                }
            }

            public GameObject gameObject;
            public List<Component> components = new();
        }

        public CommitDiff Diff
        {
            get
            {
                if (_commitDiff == null) CacheCommitDiff();
                return _commitDiff;
            }
        }

        private CommitDiff _commitDiff;

        public readonly ulong targetId;
        public readonly GitCommit src;
        public readonly GitCommit dest;

        public readonly UnityYamlDocumentWithExtra srcObject;
        public readonly UnityYamlDocumentWithExtra destObject;

        public bool IsNotExist { get => srcObject == null && destObject == null; }

        public bool IsSame
        {
            get
            {
                if (IsNotExist) return true;
                return srcObject?.Equals(destObject) ?? false;
            }
        }

        public ObjectCommitDiff(ulong targetId, GitCommit src, GitCommit dest)
        {
            this.targetId = targetId;
            this.src = src;
            this.dest = dest;

            if (src.unityYaml.TryGetGameObject(targetId, out var srcYaml)) srcObject = srcYaml;
            if (dest.unityYaml.TryGetGameObject(targetId, out var destYaml)) destObject = destYaml;
        }

        private void CacheCommitDiff()
        {
            _commitDiff = new();

            if (srcObject == null && destObject == null) { }
            else if (srcObject == null && destObject != null)
                _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.GameObjectState.Add };
            else if (srcObject != null && destObject == null)
                _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.GameObjectState.Destroy };
            else if (srcObject.Stripped && destObject.IsGameObject)
                _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.GameObjectState.PrefabToGameObject };
            else if (srcObject.IsGameObject && destObject.Stripped)
                _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.GameObjectState.GameObjectToPrefab };
            else if (srcObject.Stripped && destObject.Stripped)
            {
                // TODO prefabの比較
                if (srcObject.IsVirtual ^ destObject.Stripped)
                {
                    _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.GameObjectState.Change };
                }
                else if (srcObject.IsVirtual)
                { }
                else
                {
                    if (srcObject.document != destObject.document)
                        _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.GameObjectState.Change };
                }
            }
            else if (srcObject.IsGameObject && destObject.IsGameObject)
            {
                var srcComponents = srcObject.components;
                var destComponents = destObject.components;
                foreach (var sameKey in Enumerable.Intersect(srcObject.components.Keys, destObject.components.Keys))
                {
                    if (!srcObject.components[sameKey].Equals(destObject.components[sameKey]))
                    {
                        _commitDiff.components.Add(new CommitDiff.Component(CommitDiff.ComponentState.Change, srcObject.components[sameKey], destComponents[sameKey]));
                    }
                }

                foreach (var addKey in destObject.components.Keys.Except(srcObject.components.Keys))
                {
                    _commitDiff.components.Add(new CommitDiff.Component(CommitDiff.ComponentState.Add, null, destComponents[addKey]));
                }
                foreach (var destroyKey in srcObject.components.Keys.Except(destObject.components.Keys))
                {
                    _commitDiff.components.Add(new CommitDiff.Component(CommitDiff.ComponentState.Destroy, srcComponents[destroyKey], null));
                }

                if (srcObject.document != destObject.document)
                {
                    _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.GameObjectState.Change };
                }
            }
        }
    }
}
