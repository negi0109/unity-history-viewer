using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class GameObjectYaml
    {
        public string name;
        public List<int> componentIds = new();

        public GameObjectYaml(UnityYamlDocument doc, ILogger logger = null)
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
                        if (logger != null) logger.Log("parse:end components");
                        isComponents = false;
                    }
                }

                if (Regex.IsMatch(line, ".*m_Component:.*"))
                {
                    if (logger != null) logger.Log("parse:start components");

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
