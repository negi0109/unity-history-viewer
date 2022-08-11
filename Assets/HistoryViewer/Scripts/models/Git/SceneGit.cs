using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class SceneGit
    {
        private readonly IGitCommandExecutor _git;
        private readonly IFileLoader _fileLoader;
        private readonly ILogger _logger;
        private readonly string _scenePath;
        private readonly Dictionary<string, UnityYamlDocument> _unityYamlDocumentPool = new();
        public List<GitCommit> commits;

        public SceneGit(IGitCommandExecutor gce, IFileLoader fileLoader, string scenePath, ILogger logger = null)
        {
            _git = gce;
            _fileLoader = fileLoader;
            _scenePath = scenePath;
            _logger = logger;
        }

        public void LoadGitHistory()
        {
            var loader = new GitLogLoader(_git, _logger);
            commits = loader.Load(_scenePath);

            // 現在のファイルの状況を取得
            commits.Insert(0, new GitCommit(null, "Current"));

            // コミットのファイルの取得
            foreach (var commit in commits)
            {
                if (commit.IsLocalFile)
                {
                    _fileLoader.LoadFile(
                        _scenePath,
                        reader =>
                        {
                            var parser = new UnityYamlParser(_unityYamlDocumentPool, _logger);
                            commit.unityYaml = parser.Parse(reader);
                        }
                    );
                }
                else
                {
                    _git.ExecGitCommand(
                        $"show {commit.hashId}:\"{_scenePath}\"",
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
}