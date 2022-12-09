using System.Collections.Generic;

namespace Negi0109.HistoryViewer.Models
{
    public class GUIDDatabase
    {
        public Dictionary<string, string> dic = new();

        public bool TryGetValue(string guid, out string value)
        {
            if (dic.TryGetValue(guid, out string v))
            {
                value = v;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}
