using System;
using System.IO;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Middleware
{
    public class GitCommandExecutor : IGitCommandExecutor
    {
        private readonly ILogger _logger;

        public GitCommandExecutor(ILogger logger = null)
        {
            _logger = logger;
        }

        public string ExecGitCommand(string arguments)
        {
            if (_logger != null) _logger.Log($"exec-git: {arguments}");
            return Utils.GitCommandUtil.ExecGitCommand(arguments);
        }

        public void ExecGitCommand(string arguments, Action<StreamReader> func)
        {
            if (_logger != null) _logger.Log($"exec-git: {arguments}");
            Utils.GitCommandUtil.ExecGitCommand(arguments, func);
        }
    }
}
