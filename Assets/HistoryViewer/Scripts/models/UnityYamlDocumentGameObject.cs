using System;
using System.Text.RegularExpressions;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYamlDocumentGameObject
    {
        public string name;

        public UnityYamlDocumentGameObject(UnityYamlDocument doc)
        {
            foreach (var line in doc.content.Split("\n"))
            {
                if (Regex.IsMatch(line, " *m_Name: .*"))
                {
                    name = line.Split("m_Name: ", 2)[1];
                }
            }
        }
    }
}
