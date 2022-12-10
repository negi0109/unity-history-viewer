using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Negi0109.HistoryViewer.Interfaces;

public class EditorCacheStub : IEditorCache
{
    private Dictionary<string, string> dic = new();

    public bool Exists(string label, string key)
    {
        return dic.ContainsKey(label + "/" + key);
    }

    public string Get(string label, string key)
    {
        return dic[label + "/" + key];
    }

    public void Get(string label, string key, Action<StreamReader> func)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(Get(label, key)));
        var stubReader = new StreamReader(stream);

        func(stubReader);
        stubReader.Close();
    }

    public void Put(string label, string key, string content)
    {
        dic[label + "/" + key] = content;
    }
}
