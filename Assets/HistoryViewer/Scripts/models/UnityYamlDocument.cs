using System;
using System.Collections.Generic;
using Negi0109.HistoryViewer.Interfaces;

namespace Negi0109.HistoryViewer.Models
{
    /// <summary>
    /// UnityYamlをドキュメント単位で解析し結果を格納するValueObject
    /// </summary>
    /// <remarks>
    /// 他のドキュメントをもとに付与する情報は<see cref="UnityYamlDocumentExtra"/>で管理予定
    /// </remarks>
    public class UnityYamlDocument
    {
        public bool IsHeader { get => name == null; }
        public bool IsAnyObject { get => !IsHeader && !IsGameObject; }
        public bool IsGameObject
        {
            get => type == 1;
        }
        public bool IsPrefab { get => type == 1001; }

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

        public PrefabYaml PrefabObject
        {
            get
            {
                CacheContent();
                if (IsPrefab) return _prefabObject;

                return null;
            }
        }

        public StrippedGameObjectYaml StrippedGameObject
        {
            get
            {
                CacheContent();

                if (IsGameObject && Stripped) return _strippedGameObject;
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
        public readonly ComponentType componentType;

        public readonly int type;
        private readonly ulong _fileId;
        private readonly bool _stripped;

        private bool _contentCached = false;
        private GameObjectYaml _gameObject;
        private AnyYaml _anyObject;
        private PrefabYaml _prefabObject;
        private StrippedGameObjectYaml _strippedGameObject;

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
                    this.type = type;
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

            if (IsHeader) componentType = ComponentType.Header;
            else if (IsPrefab) componentType = ComponentType.PrefabInstance;
            else if (IsGameObject && Stripped) componentType = ComponentType.StrippedGameObject;
            else if (IsGameObject) componentType = ComponentType.GameObject;
            else if (IsAnyObject) componentType = ComponentType.OtherComponent;
        }

        private void CacheContent()
        {
            if (_contentCached) return;
            if (IsHeader) { }
            else if (IsPrefab)
            {
                _prefabObject = new PrefabYaml(this);
            }
            else if (IsGameObject && Stripped)
            {
                _strippedGameObject = new StrippedGameObjectYaml(this);
            }
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
                _pool = pool ?? new();
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
