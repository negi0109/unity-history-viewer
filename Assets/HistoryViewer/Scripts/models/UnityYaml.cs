using System.Collections.Generic;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYaml
    {
        public UnityYamlDocument header;
        public Dictionary<int, UnityYamlDocument> gameObjectDocuments = new();
        public Dictionary<int, UnityYamlDocument> anyObjectDocumentDictionary = new();

        public bool TryGetGameObject(int id, out UnityYamlDocument yaml)
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

        public bool TryGetComponent(int componentId, out UnityYamlDocument yaml)
        {
            if (anyObjectDocumentDictionary.TryGetValue(componentId, out var value))
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
            if (document.IsGameObject) gameObjectDocuments.Add(document.FileId, document);
            else if (document.IsAnyObject) anyObjectDocumentDictionary.Add(document.FileId, document);
            else if (document.IsHeader) header = document;
        }
    }
}
