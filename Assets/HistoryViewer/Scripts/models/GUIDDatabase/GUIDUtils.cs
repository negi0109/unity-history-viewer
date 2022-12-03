using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Negi0109.HistoryViewer.Models
{
    public static class GUIDUtils
    {
        public static string GetFileGUID(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (YamlUtils.IsKey(line, "guid"))
                {
                    return YamlUtils.GetInlineValue(line, "guid");
                }
            }

            return null;
        }
    }
}
