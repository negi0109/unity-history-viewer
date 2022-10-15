using System;
using NUnit.Framework;
using Negi0109.HistoryViewer.Models;
using Negi0109.HistoryViewer.Interfaces;

public class GUIDDatabaseCacheLoaderTest
{
    private static IEditorCache _cache;
    private static GUIDDatabaseCacheLoader _loader;

    [SetUp]
    public void SetUP()
    {
        _cache = new EditorCacheStub();
        _loader = new GUIDDatabaseCacheLoader(_cache);
    }
    [TestCase("key", "id", "path")]
    public void Save1(string key, string id, string path)
    {
        GUIDDatabase db = new();
        db.dic.Add(id, path);
        _loader.Put(key, db);

        var content = _cache.Get(GUIDDatabaseCacheLoader.cacheLabel, key);
        var array = content.Split(Environment.NewLine);

        Assert.That(array[0], Is.EqualTo($"{id},{path}"));
    }

    [TestCase("key", "id", "path", "id2", "path2")]
    public void Save2(string key, string id, string path, string id2, string path2)
    {
        GUIDDatabase db = new();
        db.dic.Add(id, path);
        db.dic.Add(id2, path2);
        _loader.Put(key, db);

        var content = _cache.Get(GUIDDatabaseCacheLoader.cacheLabel, key);
        var array = content.Split(Environment.NewLine);

        Assert.That(array[0], Is.EqualTo($"{id},{path}"));
        Assert.That(array[1], Is.EqualTo($"{id2},{path2}"));
    }

    [TestCase("key", "id,path", "id", "path")]
    public void Get1(string key, string text, string id, string path)
    {
        _cache.Put(GUIDDatabaseCacheLoader.cacheLabel, key, text);
        var db = _loader.Get(key);
        Assert.That(db.dic[id], Is.EqualTo(path));
    }

    [TestCase("key", "id,path\nid2,path2", "id", "path", "id2", "path2")]
    public void Get2(string key, string text, string id, string path, string id2, string path2)
    {
        _cache.Put(GUIDDatabaseCacheLoader.cacheLabel, key, text);
        var db = _loader.Get(key);
        Assert.That(db.dic[id], Is.EqualTo(path));
        Assert.That(db.dic[id2], Is.EqualTo(path2));
    }

    [TestCase("key", "id,path")]
    public void Exist(string key, string text)
    {
        _cache.Put(GUIDDatabaseCacheLoader.cacheLabel, key, text);

        Assert.That(_loader.Exists(key), Is.EqualTo(true));
    }

    [TestCase("key")]
    public void NotExist(string key)
    {
        Assert.That(_loader.Exists(key), Is.EqualTo(false));
    }
}
