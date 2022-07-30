using System.Collections.Generic;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYaml
    {
        public List<UnityYamlDocument> documents = new();
        public Dictionary<int, UnityYamlDocument> _gameObjectDocuments = new();
        public Dictionary<int, UnityYamlDocument> _anyObjectDocumentDictionary = new();

        public bool TryGetGameObject(int id, out UnityYamlDocument yaml)
        {
            if (_gameObjectDocuments.TryGetValue(id, out var value))
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
            if (_anyObjectDocumentDictionary.TryGetValue(componentId, out var value))
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
            documents.Add(document);

            if (document.IsGameObject) _gameObjectDocuments.Add(document.FileId, document);
            else if (document.IsAnyObject) _anyObjectDocumentDictionary.Add(document.FileId, document);
        }
    }
}
