using System.Collections.Generic;

public class GitLogLoader
{
    private readonly int _logMax = 30;
    private readonly string _target;
    private readonly IGitCommandExecutor _git;
    private readonly ILogger _logger;

    public GitLogLoader(string target, IGitCommandExecutor git, ILogger logger)
    {
        _target = target;
        _git = git;
        _logger = logger;
    }

    public List<GitCommit> Load()
    {
        var commits = new List<GitCommit>();
        var commitParser = new GitCommitParser();

        var history = _git.ExecGitCommand($"log -n {_logMax} --pretty=\"{GitCommitParser.LogFormat}\" -- {_target}");
        _logger.Log(history);

        foreach (var line in history.Split("\n"))
        {
            if (string.IsNullOrEmpty(line)) continue;

            var commit = commitParser.Parse(line);

            commits.Add(commit);
        }

        return commits;
    }
}
