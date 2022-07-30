using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class GitLogLoader
    {
        private readonly int _logMax = 30;
        private readonly IGitCommandExecutor _git;
        private readonly ILogger _logger;

        public GitLogLoader(IGitCommandExecutor git, ILogger logger = null)
        {
            _git = git;
            _logger = logger;
        }

        public List<GitCommit> Load(string target)
        {
            var commits = new List<GitCommit>();
            var commitParser = new GitCommitParser();

            _git.ExecGitCommand(
                $"log -n {_logMax} --pretty=\"{GitCommitParser.LogFormat}\" -- {target}",
                reader =>
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var commit = commitParser.Parse(line);

                        commits.Add(commit);
                    }
                }
            );

            return commits;
        }
    }
}
