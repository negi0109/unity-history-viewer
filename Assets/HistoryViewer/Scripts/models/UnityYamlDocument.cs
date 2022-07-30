using System;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYamlDocument
    {
        public bool IsHeader { get => name == null; }
        public bool IsAnyObject { get => !IsHeader && !IsGameObject; }
        public bool IsGameObject
        {
            get
            {
                CacheName();

                return type == 1;
            }
        }

        public GameObjectYaml GameObject
        {
            get
            {
                CacheContent();
                if (IsGameObject) return _gameObject;

                return null;
            }
        }

        public AnyYaml AnyObject
        {
            get
            {
                CacheContent();
                if (IsGameObject) return _anyObject;

                return null;
            }
        }

        public readonly string name;
        public readonly string content;
        public bool nameCached = false;
        public bool contentCached = false;
        public int type;
        private GameObjectYaml _gameObject;
        private AnyYaml _anyObject;

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
            if (IsHeader) { }
            else if (IsGameObject)
            {
                _gameObject = new GameObjectYaml(this);
            }
            else if (IsAnyObject)
            {
                _anyObject = new AnyYaml(this);
            }

            contentCached = true;
        }
    }
}
