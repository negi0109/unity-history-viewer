using System;
using System.Collections.Generic;

namespace Negi0109.HistoryViewer.Models
{
    public class GameObjectYaml
    {
        public string name;
        public List<ulong> componentIds = new();

        public GameObjectYaml(UnityYamlDocument doc)
        {
            bool isComponents = false;

            foreach (var line in doc.content.Split("\n"))
            {
                if (isComponents)
                {
                    if (YamlUtils.IsArrayElement(line))
                    {
                        var fileID = YamlUtils.GetBlockValue(YamlUtils.GetInlineValue(line, "- component"), "fileID");
                        if (ulong.TryParse(fileID, out ulong id)) componentIds.Add(id);
                        else throw new FormatException();

                        continue;
                    }
                    else
                    {
                        isComponents = false;
                    }
                }

                var key = YamlUtils.GetInlineKey(line);
                if (key is null) continue;

                if (!isComponents && key.Equals("m_Component"))
                {
                    isComponents = true;
                    continue;
                }

                if (name == null && key.Equals("m_Name"))
                {
                    name = YamlUtils.GetInlineValue(line, "m_Name");
                }
            }
        }
    }
}
