using System;
using System.Text.RegularExpressions;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYamlDocument
    {
        public bool IsGameObject
        {
            get
            {
                CacheName();

                return type == 1;
            }
        }
        public string GameObjectName
        {
            get
            {
                CacheContent();

                return gameObjectName;
            }
        }

        public readonly string name;
        public readonly string content;
        public bool nameCached = false;
        public bool contentCached = false;
        public int type;
        public string gameObjectName;

        public UnityYamlDocument(string name, string content)
        {
            this.name = name;
            this.content = content;
        }

        private void CacheName()
        {
            if (nameCached) return;
            if (name == null) return;

            nameCached = true;

            var attributes = name.Split(" ")[0].Split("!");
            if (attributes[1] != "u") throw new FormatException();

            if (int.TryParse(attributes[2], out int num))
            {
                type = num;
            }
            else
            {
                throw new FormatException();
            }
        }

        private void CacheContent()
        {
            if (contentCached) return;

            if (IsGameObject)
            {
                // m_Name取得
                foreach (var line in content.Split("\n"))
                {
                    if (Regex.IsMatch(line, " *m_Name: .*"))
                    {
                        gameObjectName = line.Split(":", 2)[1];
                    }
                }
            }

            contentCached = true;
        }
    }
}
