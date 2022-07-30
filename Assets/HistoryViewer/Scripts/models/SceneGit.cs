using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class SceneGit
    {
        private readonly IGitCommandExecutor _git;
        private readonly ILogger _logger;
        private readonly string _scenePath;

        public SceneGit(IGitCommandExecutor gce, string scenePath, ILogger logger = null)
        {
            _git = gce;
            _scenePath = scenePath;
            _logger = logger;
        }

        public void LoadGitHistory()
        {
            var loader = new GitLogLoader(_git, _logger);
            var commits = loader.Load(_scenePath);
        }
    }
}
