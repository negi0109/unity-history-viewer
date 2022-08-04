using System;
using System.IO;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Middleware
{
    public class FileLoader : IFileLoader
    {
        private readonly ILogger _logger;

        public FileLoader(ILogger logger = null)
        {
            _logger = logger;
        }

        public string LoadFile(string path)
        {
            string text = "";
            LoadFile(path, (reader) => { text = reader.ReadToEnd(); });

            return text;
        }

        public void LoadFile(string path, Action<StreamReader> func)
        {
            if (_logger != null) _logger.Log($"open-file: {path}");

            var stream = File.OpenRead(path);
            var fileReader = new StreamReader(stream);
            func(fileReader);
            stream.Close();
        }
    }
}
