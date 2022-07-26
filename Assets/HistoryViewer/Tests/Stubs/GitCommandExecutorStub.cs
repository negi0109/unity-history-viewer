using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Negi0109.HistoryViewer.Interfaces;

public class GitCommandExecutorStub : IGitCommandExecutor
{
    private readonly Dictionary<string, string> _dic;

    public GitCommandExecutorStub(Dictionary<string, string> dic = null)
    {
        dic ??= new Dictionary<string, string>();
        _dic = dic;
    }

    public string ExecGitCommand(string arguments)
    {
        foreach (var pattern in _dic.Keys)
        {
            if (Regex.IsMatch(arguments, pattern)) return _dic[pattern];
        }

        throw new Exception();
    }

    public void ExecGitCommand(string arguments, Action<StreamReader> func)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(ExecGitCommand(arguments)));
        var stubReader = new StreamReader(stream);

        func(stubReader);
        stubReader.Close();
    }
}
