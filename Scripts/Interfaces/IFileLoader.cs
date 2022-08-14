using System;
using System.IO;

namespace Negi0109.HistoryViewer.Interfaces
{
    public interface IFileLoader
    {
        public string LoadFile(string path);
        public void LoadFile(string path, Action<StreamReader> func);
    }
}
