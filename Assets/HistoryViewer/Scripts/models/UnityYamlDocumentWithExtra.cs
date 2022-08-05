
using System.Collections.Generic;

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

        public readonly List<UnityYamlDocument> components;
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
            components.Add(component.document);
        }

        public void DissolveHasOneStrippedGameObject(UnityYamlDocumentWithExtra gameObject)
        {
            strippedGameObject = gameObject;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;

            if (!document.Equals(document)) return false;

            return true;
        }
    }
}
