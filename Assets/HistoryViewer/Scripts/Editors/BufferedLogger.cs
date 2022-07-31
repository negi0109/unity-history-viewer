using System.Collections.Generic;
using System.Text;

namespace Negi0109.HistoryViewer.Editors
{
    public class BufferedLogger : Interfaces.ILogger
    {
        private Interfaces.ILogger _logger;
        private Queue<string> _logs = new();

        public BufferedLogger(Interfaces.ILogger logger)
        {
            _logger = logger;
        }

        public void Log(string text)
        {
            _logs.Enqueue(text);
        }

        public void PrintLog(string title = null)
        {
            var log = new StringBuilder();
            var logged = false;

            if (title != null) log.AppendLine(title);

            while (_logs.TryDequeue(out var text))
            {
                log.AppendLine(text);
                logged = true;
            }

            if (logged)
            {
                _logger.Log(log.ToString());
            }
        }
    }
}
