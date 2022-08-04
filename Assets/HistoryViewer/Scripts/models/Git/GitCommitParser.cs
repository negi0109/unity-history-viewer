using System;

namespace Negi0109.HistoryViewer.Models
{
    public class GitCommitParser
    {
        public const string LogFormat = "%H%x09%s";

        public GitCommit Parse(string line)
        {
            var tokens = line.Split("\t");
            if (tokens.Length != 2) throw new FormatException();

            return new GitCommit(tokens[0], tokens[1]);
        }
    }
}
