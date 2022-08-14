using System;
using System.Collections.Generic;

namespace Negi0109.HistoryViewer.Models
{
    public class StrippedGameObjectYaml
    {
        public readonly ulong prefabId;

        public StrippedGameObjectYaml(UnityYamlDocument doc)
        {
            foreach (var line in doc.content.Split("\n"))
            {
                if (YamlUtils.IsKey(line, "m_PrefabInstance"))
                {
                    var fileID = YamlUtils.GetBlockValue(YamlUtils.GetInlineValue(line, "m_PrefabInstance"), "fileID");

                    if (ulong.TryParse(fileID, out ulong id)) prefabId = id;
                    else throw new FormatException();
                }
            }
            if (prefabId == 0) throw new FormatException();
        }
    }
}
