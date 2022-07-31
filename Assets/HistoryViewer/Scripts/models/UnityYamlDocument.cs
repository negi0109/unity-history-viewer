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
            get
            {
                CacheName();

                return _type == 1;
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
                if (IsAnyObject) return _anyObject;

                return null;
            }
        }
        public int FileId
        {
            get
            {
                CacheName();

                return _fileId;
            }
        }

        public readonly string name;
        public readonly string content;
        private bool _nameCached = false;
        private bool _contentCached = false;
        private int _type;
        private int _fileId;
        private GameObjectYaml _gameObject;
        private AnyYaml _anyObject;

        private UnityYamlDocument(string name, string content)
        {
            this.name = name;
            this.content = content;
        }

        private void CacheName()
        {
            if (_nameCached) return;
            if (name == null) return;

            _nameCached = true;
            var attributes = name[..name.IndexOf(' ')].Split("!");
            if (attributes[1] != "u") throw new FormatException();

            if (int.TryParse(attributes[2], out int type))
            {
                _type = type;
            }
            else
            {
                throw new FormatException();
            }
            if (int.TryParse(name[(name.LastIndexOf('&') + 1)..], out int fileId))
            {
                _fileId = fileId;
            }
            else
            {
                throw new FormatException();
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

        public class Builder
        {
            private readonly Dictionary<string, UnityYamlDocument> _pool;
            private readonly ILogger _logger;

            public Builder(Dictionary<string, UnityYamlDocument> pool = null, ILogger logger = null)
            {
                _pool = pool;
                _logger = logger;
            }

            public UnityYamlDocument Build(string name, string content)
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
