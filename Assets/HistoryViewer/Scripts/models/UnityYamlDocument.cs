using System;
using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    public class UnityYamlDocument
    {
        public bool IsHeader { get => name == null; }
        public bool IsAnyObject { get => !IsHeader && !IsGameObject; }
        public bool IsGameObject
        {
            get => _type == 1;
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
                if (IsAnyObject) return _anyObject;

                return null;
            }
        }
        public ulong FileId
        {
            get => _fileId;
        }
        public bool Stripped
        {
            get => _stripped;
        }

        public readonly string name;
        public readonly string content;
        private readonly int _type;
        private readonly ulong _fileId;
        private readonly bool _stripped;

        private bool _contentCached = false;
        private GameObjectYaml _gameObject;
        private AnyYaml _anyObject;

        private UnityYamlDocument(string name, string content)
        {
            this.name = name;
            this.content = content;

            if (name != null)
            {
                var attributes = name.Split(' ');
                var attribute0 = attributes[0].Split('!');
                if (attribute0[1] != "u") throw new FormatException();

                if (int.TryParse(attribute0[2], out int type))
                {
                    _type = type;
                }
                else
                {
                    throw new FormatException();
                }
                if (ulong.TryParse(attributes[1][1..], out ulong fileId))
                {
                    _fileId = fileId;
                }
                else
                {
                    throw new FormatException();
                }
                for (var i = 2; i < attributes.Length; i++)
                {
                    if (attributes[i].Equals("stripped")) _stripped = true;
                }
            }
        }

        private void CacheContent()
        {
            if (_contentCached) return;
            if (IsHeader) { }
            else if (IsGameObject)
            {
                _gameObject = new GameObjectYaml(this);
            }
            else if (IsAnyObject)
            {
                _anyObject = new AnyYaml(this);
            }

            _contentCached = true;
        }

        public class Factory
        {
            private readonly Dictionary<string, UnityYamlDocument> _pool;
            private readonly ILogger _logger;

            public Factory(Dictionary<string, UnityYamlDocument> pool = null, ILogger logger = null)
            {
                _pool = pool;
                _logger = logger;
            }

            public UnityYamlDocument Get(string name, string content)
            {
                if (_pool == null)
                {
                    return new UnityYamlDocument(name, content);
                }
                else if (_pool.TryGetValue(content, out var value))
                {
                    if (_logger != null) _logger.Log("cache hit");

                    return value;
                }
                else
                {
                    if (_logger != null) _logger.Log("cache miss");

                    var doc = new UnityYamlDocument(name, content);
                    _pool.Add(content, doc);
                    return doc;
                }
            }
        }
    }
}
