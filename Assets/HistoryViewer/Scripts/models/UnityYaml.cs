using System.Collections.Generic;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYaml
    {
        public UnityYamlDocumentWithExtra header;
        public Dictionary<ulong, UnityYamlDocumentWithExtra> gameObjectDocuments = new();
        public Dictionary<ulong, UnityYamlDocumentWithExtra> anyObjectDocuments = new();

        public bool TryGetGameObject(ulong id, out UnityYamlDocumentWithExtra yaml)
        {
            if (gameObjectDocuments.TryGetValue(id, out var value))
            {
                yaml = value;
                return true;
            }
            else
            {
                yaml = null;
                return false;
            }
        }

        public bool TryGetComponent(ulong componentId, out UnityYamlDocumentWithExtra yaml)
        {
            if (anyObjectDocuments.TryGetValue(componentId, out var value))
            {
                yaml = value;
                return true;
            }
            else
            {
                yaml = null;
                return false;
            }
        }

        public void AddYamlDocument(UnityYamlDocument document)
        {
            var docEx = new UnityYamlDocumentWithExtra(document);

            if (document.IsGameObject) gameObjectDocuments.Add(document.FileId, docEx);
            else if (document.IsPrefab) gameObjectDocuments.Add(document.FileId, docEx);
            else if (document.IsAnyObject) anyObjectDocuments.Add(document.FileId, docEx);
            else if (document.IsHeader) header = docEx;
        }

        /// <summary>
        /// 双方向にGameObject, Componentを紐づける
        /// </summary>
        public void DissolveAssociations()
        {
            // Component側からGameObjectへ登録
            foreach (var component in anyObjectDocuments.Values)
            {
                var componentYaml = component.document.AnyObject;

                if (componentYaml.IsBelongsToGameObject)
                {
                    if (gameObjectDocuments.TryGetValue(componentYaml.gameObjectId, out var val))
                    {
                        val.DissolveHasManyGameObject(component);
                    }
                }
            }

            foreach (var gameObject in gameObjectDocuments.Values)
            {
                if (gameObject.Stripped)
                {
                    if (gameObjectDocuments.TryGetValue(gameObject.document.StrippedGameObject.prefabId, out var prefab))
                    {
                        prefab.strippedGameObject = gameObject;
                    }
                }
            }
        }
    }
}
