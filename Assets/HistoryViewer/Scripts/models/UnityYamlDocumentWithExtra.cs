
using System.Collections.Generic;
using System.Linq;

namespace Negi0109.HistoryViewer.Models
{
    /// <summary>
    /// UnityYamlドキュメントとその他ドキュメントで構成される情報を持つオブジェクト
    /// </summary>
    /// <remarks>
    /// GameObjectに対するコンポーネントなど複数ドキュメントにまたがるものをここで解決する
    /// </remarks>
    public class UnityYamlDocumentWithExtra
    {
        public readonly UnityYamlDocument document;

        public readonly Dictionary<ulong, UnityYamlDocument> components;
        public UnityYamlDocumentWithExtra strippedGameObject;

        public bool IsPrefab { get => document.IsPrefab; }
        public bool IsAnyObject { get => document.IsAnyObject; }
        public bool IsGameObject { get => document.IsGameObject; }
        public bool Stripped { get => document.Stripped; }

        public UnityYamlDocumentWithExtra(UnityYamlDocument document)
        {
            this.document = document;
            if (IsGameObject) components = new();
        }

        public void DissolveHasManyGameObject(UnityYamlDocumentWithExtra component)
        {
            components.Add(component.document.FileId, component.document);
        }

        public void DissolveHasOneStrippedGameObject(UnityYamlDocumentWithExtra gameObject)
        {
            strippedGameObject = gameObject;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            var another = (UnityYamlDocumentWithExtra)obj;

            if (document != another.document) return false;

            foreach (var sameKey in Enumerable.Intersect(components.Keys, another.components.Keys))
            {
                if (!components[sameKey].Equals(another.components[sameKey]))
                {
                    return false;
                }
            }

            if (another.components.Keys.Except(components.Keys).Any()) return false;
            if (components.Keys.Except(another.components.Keys).Any()) return false;

            return true;
        }
    }
}
