using System;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYamlDocument
    {
        public bool IsGameObject
        {
            get
            {
                if (!nameCached) CacheName();

                return type == 1;
            }
        }

        public readonly string name;
        public readonly string content;
        public bool nameCached = false;
        public int type;

        public UnityYamlDocument(string name, string content)
        {
            this.name = name;
            this.content = content;
        }

        private void CacheName()
        {
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
    }
}
