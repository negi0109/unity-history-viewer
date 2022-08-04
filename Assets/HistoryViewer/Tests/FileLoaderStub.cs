using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Negi0109.HistoryViewer.Interfaces;

public class FileLoaderStub : IFileLoader
{
    private readonly Dictionary<string, string> _dic;

    public FileLoaderStub(Dictionary<string, string> dic)
    {
        _dic = dic;
    }

    public string LoadFile(string path)
    {
        if (_dic.TryGetValue(path, out var val))
        {
            return val;
        }
        else
        {
            throw new Exception();
        }
    }

    public void LoadFile(string path, Action<StreamReader> func)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(LoadFile(path)));
        var stubReader = new StreamReader(stream);

        func(stubReader);
        stubReader.Close();
    }
}
