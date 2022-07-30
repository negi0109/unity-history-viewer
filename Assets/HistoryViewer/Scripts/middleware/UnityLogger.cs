using UnityEngine;

namespace Negi0109.HistoryViewer.Middleware
{
    public class UnityLogger : Interfaces.ILogger
    {
        public void Log(string text)
        {
            Debug.Log(text);
        }
    }
}
