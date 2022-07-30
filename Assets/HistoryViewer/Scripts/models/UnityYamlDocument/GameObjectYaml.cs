using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Negi0109.HistoryViewer.Models
{
    public class GameObjectYaml
    {
        public string name;
        public List<int> componentIds = new();

        public GameObjectYaml(UnityYamlDocument doc)
        {
            bool isComponents = false;

            foreach (var line in doc.content.Split("\n"))
            {
                if (isComponents)
                {
                    if (line.IndexOf("- component: ") != -1)
                    {
                        if (int.TryParse(Regex.Match(line, @"\d+").Value, out int id))
                            componentIds.Add(id);
                        else throw new FormatException();

                        continue;
                    }
                    else
                    {
                        isComponents = false;
                    }
                }

                if (!isComponents && line.IndexOf("m_Component:") != -1)
                {
                    isComponents = true;
                    continue;
                }

                if (name == null && line.IndexOf("m_Name: ") != -1)
                {
                    name = line[(line.IndexOf(":") + 2)..];
                }
            }
        }
    }
}
