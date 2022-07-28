using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Text;

public static class GitCommandUtil
{
    private static string gitPath = null;

    private static string GetGitPath()
    {
        if (gitPath != null) return gitPath;

        gitPath = "git";

        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            foreach (var path in new string[]{
                "/usr/local/bin/git",
                "/usr/bin/git"
                }
            )
            {
                if (File.Exists(path))
                {
                    gitPath = path;
                }
            }
        }

        return gitPath;
    }

    public static string ExecGitCommand(string arguments)
    {
        var psi = new ProcessStartInfo()
        {
            UseShellExecute = false,
            CreateNoWindow = false,
            RedirectStandardOutput = true,
            StandardOutputEncoding = Encoding.UTF8,
            Arguments = arguments,
            FileName = GetGitPath(),
        };

        return ProcessUtil.ExecProcess(psi);
    }
}
