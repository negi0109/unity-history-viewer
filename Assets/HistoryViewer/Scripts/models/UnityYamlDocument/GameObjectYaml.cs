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
            var lines = doc.content.Split("\n");
            var len = lines.Length;
            for (int i = 0; i < len; i++)
            {
                var line = lines[i];
                var lineIndex = YamlUtils.GetIndentSize(line);
                var key = YamlUtils.GetInlineKey(line, lineIndex);

                if (string.IsNullOrEmpty(key)) continue;
                else if (key.Equals("m_Component"))
                {
                    i++;
                    for (; i < len; i++)
                    {
                        var componentLine = lines[i];
                        var valueIndent = YamlUtils.GetIndentSize(componentLine);
                        if (lineIndex == valueIndent && YamlUtils.IsArrayElement(componentLine, valueIndent))
                        {
                            var fileID = YamlUtils.GetBlockValue(YamlUtils.GetInlineValue(componentLine, "- component", valueIndent), "fileID");
                            componentIds.Add(ulong.Parse(fileID));
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (key.Equals("m_Name"))
                {
                    name = YamlUtils.GetInlineValue(line, "m_Name");
                }
            }
        }
    }
}
