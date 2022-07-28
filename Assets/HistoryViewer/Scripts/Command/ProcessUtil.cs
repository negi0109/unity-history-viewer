using System.Diagnostics;

public static class ProcessUtil
{
    public static string ExecProcess(string command)
    {
        var ps = new ProcessStartInfo(command)
        {
            UseShellExecute = false,
            CreateNoWindow = false,
            RedirectStandardOutput = true,
        };

        using var proc = new Process()
        {
            StartInfo = ps,
        };

        proc.Start();
        var text = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();

        return text;
    }
}
