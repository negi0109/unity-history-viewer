namespace Negi0109.HistoryViewer.Models
{
    public class ObjectCommitDiff
    {
        public class CommitDiff
        {
            public enum State
            {
                Add,
                Destroy,
                Change
            }

            public class GameObject
            {
                public State state;
            }

            public GameObject gameObject;
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

        public bool IsSame
        {
            get
            {
                if (srcObject == null && destObject == null) return true;
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
                _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.State.Add };
            else if (srcObject != null && destObject == null)
                _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.State.Destroy };
            else if (srcObject != null && destObject != null)
            {
                if (srcObject.document != destObject.document)
                {
                    _commitDiff.gameObject = new CommitDiff.GameObject() { state = CommitDiff.State.Change };
                }
            }
        }
    }
}
