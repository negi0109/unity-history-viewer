using System.Collections.Generic;
using System.Text;

namespace Negi0109.HistoryViewer.Editors
{
    public class BufferedLogger : Interfaces.ILogger
    {
        private readonly Interfaces.ILogger _logger;
        private readonly bool _composite;
        private readonly Queue<string> _logs = new();

        public BufferedLogger(Interfaces.ILogger logger, bool composite = false)
        {
            _logger = logger;
            _composite = composite;
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

            if (_composite)
            {
                int compositeLogCount = 1;

                if (_logs.TryDequeue(out var prevLog)) logged = true;
                else return;

                while (_logs.TryDequeue(out var nextLog))
                {
                    if (prevLog.Equals(nextLog))
                    {
                        compositeLogCount++;
                    }
                    else
                    {
                        if (compositeLogCount == 1) log.AppendLine(prevLog);
                        else log.AppendLine($"{prevLog} (repeated={compositeLogCount})");

                        prevLog = nextLog;
                        compositeLogCount = 1;
                    }
                }

                if (compositeLogCount == 1) log.AppendLine(prevLog);
                else log.AppendLine($"{prevLog} (repeated={compositeLogCount})");
            }
            else
            {
                while (_logs.TryDequeue(out var text))
                {
                    log.AppendLine(text);
                    logged = true;
                }
            }

            if (logged)
            {
                _logger.Log(log.ToString());
            }
        }
    }
}
