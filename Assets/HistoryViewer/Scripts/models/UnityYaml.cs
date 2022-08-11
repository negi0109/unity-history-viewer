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
            else if (document.IsPrefab) anyObjectDocuments.Add(document.FileId, docEx);
            else if (document.IsAnyObject) anyObjectDocuments.Add(document.FileId, docEx);
            else if (document.IsHeader) header = docEx;
        }

        /// <summary>
        /// 双方向にGameObject, Componentを紐づける
        /// </summary>
        public void DissolveAssociations()
        {
            Dictionary<ulong, UnityYamlDocumentWithExtra> tmpGameObjectDocuments = new();
            foreach (var gameObject in gameObjectDocuments.Values)
            {
                if (gameObject.Stripped)
                {
                    if (anyObjectDocuments.TryGetValue(gameObject.document.StrippedGameObject.prefabId, out var prefab))
                    {
                        gameObject.SetPrefabInstance(prefab);

                        var strippedGameObjectId = prefab.document.FileId ^ prefab.document.PrefabObject.correspondingSourceObjectId;
                        tmpGameObjectDocuments.Add(strippedGameObjectId, gameObject);
                    }
                }
            }
            foreach (var tmp in tmpGameObjectDocuments)
            {
                // StrippedGameObjectがない場合のみ仮想のものを登録
                if (!gameObjectDocuments.ContainsKey(tmp.Key))
                    gameObjectDocuments.Add(tmp.Key, tmp.Value);
            }


            // Component側からGameObjectへ登録
            foreach (var component in anyObjectDocuments.Values)
            {
                if (component.IsPrefab)
                {
                    if (component.strippedGameObject == null)
                    {
                        var strippedObject = new UnityYamlDocumentWithExtra(null);
                        var strippedGameObjectId = component.document.FileId ^ component.document.PrefabObject.correspondingSourceObjectId;

                        strippedObject.SetPrefabInstance(component);
                        gameObjectDocuments.Add(strippedGameObjectId, strippedObject);
                    }
                }
                else if (component.IsAnyObject)
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
            }
        }
    }
}
