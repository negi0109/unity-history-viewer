using System;
using System.IO;

namespace Negi0109.HistoryViewer.Interfaces
{
    public interface IEditorCache
    {
        public bool Exists(string label, string key);
        public void Put(string label, string key, string content);
        public string Get(string label, string key);
        public void Get(string label, string key, Action<StreamReader> func);
    }
}
