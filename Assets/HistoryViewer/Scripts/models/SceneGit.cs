public class SceneGit
{
    private readonly IGitCommandExecutor _git;
    private readonly ILogger _logger = new ILogger.NoLogger();
    private readonly string _scenePath;

    public SceneGit(IGitCommandExecutor gce, string scenePath, ILogger logger = null)
    {
        _git = gce;
        _scenePath = scenePath;

        if (logger != null) _logger = logger;
    }

    public void LoadGitHistory()
    {
        var loader = new GitLogLoader(_git, _logger);
        var commits = loader.Load(_scenePath);
    }
}
