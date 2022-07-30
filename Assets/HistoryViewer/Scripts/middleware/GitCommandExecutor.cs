using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Middleware
{
    public class GitCommandExecutor : IGitCommandExecutor
    {
        public string ExecGitCommand(string arguments)
        {
            return GitCommandUtil.ExecGitCommand(arguments);
        }
    }
}
