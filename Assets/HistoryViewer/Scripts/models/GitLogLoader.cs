using System.Collections.Generic;

public class GitLogLoader
{
    private readonly int _logMax = 30;
    private readonly IGitCommandExecutor _git;
    private readonly ILogger _logger = new ILogger.NoLogger();

    public GitLogLoader(IGitCommandExecutor git, ILogger logger = null)
    {
        _git = git;
        if (logger != null) _logger = logger;
    }

    public List<GitCommit> Load(string target)
    {
        var commits = new List<GitCommit>();
        var commitParser = new GitCommitParser();

        var history = _git.ExecGitCommand($"log -n {_logMax} --pretty=\"{GitCommitParser.LogFormat}\" -- {target}");
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
