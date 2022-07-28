public class SceneGit
{
    private static int logMax = 30;
    private IGitCommandExecutor _git;
    private ILogger _logger = new ILogger.NoLogger();
    private string _scenePath;

    public SceneGit(IGitCommandExecutor gce, string scenePath, ILogger logger = null)
    {
        _git = gce;
        _scenePath = scenePath;

        if (logger != null) _logger = logger;
    }

    public void LoadGitHistory()
    {
        var history = _git.ExecGitCommand($"log -n {logMax} --pretty=\"%H%x09%s\" -- {_scenePath}");
        _logger.Log(history);

    }
}
