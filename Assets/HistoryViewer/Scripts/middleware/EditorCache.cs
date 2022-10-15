using System.IO;
using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;
using UnityEngine;

namespace Negi0109.HistoryViewer.Middleware
{
    public class EditorCache : IEditorCache
    {
        private HashSet<string> labels = new();
        private string basePath = Application.dataPath + "/../Library/negi0109.history-viewer/caches";

        public EditorCache()
        {
            Directory.CreateDirectory(basePath);
        }

        public bool Exists(string label, string key)
        {
            return File.Exists(basePath + "/" + label + "/" + key);
        }

        public string Get(string label, string key)
        {
            return File.ReadAllText(basePath + "/" + label + "/" + key);
        }

        public void Put(string label, string key, string content)
        {
            SetUpLabel(label);
            File.WriteAllText(basePath + "/" + label + "/" + key, content);
        }

        public void SetUpLabel(string label)
        {
            if (labels.Contains(label)) return;

            labels.Add(label);
            Directory.CreateDirectory(basePath + "/" + label);
        }
    }
}
