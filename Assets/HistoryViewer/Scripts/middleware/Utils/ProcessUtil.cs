using System.Diagnostics;

namespace Negi0109.HistoryViewer.Middleware.Utils
{
    public static class ProcessUtil
    {
        public static string ExecProcess(ProcessStartInfo psi)
        {
            using var proc = new Process()
            {
                StartInfo = psi,
            };

            proc.Start();
            var text = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            return text;
        }
    }
}
