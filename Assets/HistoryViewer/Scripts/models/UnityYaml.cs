using System.Collections.Generic;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYaml
    {
        public UnityYamlDocument header;
        public Dictionary<ulong, UnityYamlDocument> gameObjectDocuments = new();
        public Dictionary<ulong, UnityYamlDocument> anyObjectDocuments = new();

        public bool TryGetGameObject(ulong id, out UnityYamlDocument yaml)
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

        public bool TryGetComponent(ulong componentId, out UnityYamlDocument yaml)
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
            if (document.IsGameObject) gameObjectDocuments.Add(document.FileId, document);
            else if (document.IsAnyObject) anyObjectDocuments.Add(document.FileId, document);
            else if (document.IsHeader) header = document;
        }
    }
}
