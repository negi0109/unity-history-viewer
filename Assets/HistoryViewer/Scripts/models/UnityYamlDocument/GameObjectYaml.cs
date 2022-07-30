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
                    if (Regex.IsMatch(line, ".*- component: .*"))
                        if (int.TryParse(Regex.Replace(line, @"[^0-9]", ""), out int id))
                            componentIds.Add(id);
                        else throw new FormatException();
                    else
                    {
                        isComponents = false;
                    }
                }

                if (Regex.IsMatch(line, ".*m_Component:.*"))
                {
                    isComponents = true;
                }
                else
                {
                    if (Regex.IsMatch(line, ".*m_Name: .*"))
                        name = line.Split("m_Name: ", 2)[1];
                }
            }
        }
    }
}
