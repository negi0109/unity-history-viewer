using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class SceneGit
    {
        private readonly IGitCommandExecutor _git;
        private readonly ILogger _logger;
        private readonly string _scenePath;
        private readonly Dictionary<string, UnityYamlDocument> _unityYamlDocumentPool = new();
        public List<GitCommit> commits;

        public SceneGit(IGitCommandExecutor gce, string scenePath, ILogger logger = null)
        {
            _git = gce;
            _scenePath = scenePath;
            _logger = logger;
        }

        public void LoadGitHistory()
        {
            var loader = new GitLogLoader(_git, _logger);
            commits = loader.Load(_scenePath);

            // コミットのファイルの取得
            foreach (var commit in commits)
            {
                _git.ExecGitCommand(
                    $"show {commit.hashId}:{_scenePath}",
                    reader =>
                    {
                        var parser = new UnityYamlParser(_unityYamlDocumentPool, _logger);
                        commit.unityYaml = parser.Parse(reader);
                    }
                );
            }
        }
    }
}
