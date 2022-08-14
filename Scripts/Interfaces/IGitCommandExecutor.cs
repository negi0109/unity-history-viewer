using System;
using System.IO;

namespace Negi0109.HistoryViewer.Interfaces
{
    public interface IGitCommandExecutor
    {
        public string ExecGitCommand(string arguments);
        public void ExecGitCommand(string arguments, Action<StreamReader> func);
    }
}
