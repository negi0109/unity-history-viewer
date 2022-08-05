using System;
using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class ObjectCommitDiff
    {
        public readonly ulong targetId;
        public readonly GitCommit src;
        public readonly GitCommit dest;

        public readonly UnityYamlDocumentWithExtra srcObject;
        public readonly UnityYamlDocumentWithExtra destObject;

        public bool IsSame { get => srcObject?.Equals(destObject) ?? false; }

        public ObjectCommitDiff(ulong targetId, GitCommit src, GitCommit dest)
        {
            this.targetId = targetId;
            this.src = src;
            this.dest = dest;

            if (src.unityYaml.TryGetGameObject(targetId, out var srcYaml)) srcObject = srcYaml;
            if (dest.unityYaml.TryGetGameObject(targetId, out var destYaml)) destObject = destYaml;
        }
    }
}
