using System;
using System.Diagnostics;
using System.IO;

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

        public static void ExecProcess(ProcessStartInfo psi, Action<StreamReader> func)
        {
            using var proc = new Process()
            {
                StartInfo = psi,
            };

            proc.Start();
            func(proc.StandardOutput);
            proc.WaitForExit();
            proc.Close();
        }
    }
}
