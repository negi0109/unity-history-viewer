using NUnit.Framework;
using Negi0109.HistoryViewer.Models;

public class UnityYamlDocumentParserTest
{
    private UnityYamlDocument.Factory _factory;

    [SetUp]
    public void SetUp()
    {
        _factory = new UnityYamlDocument.Factory();
    }

    public class ParseType
    {
        private UnityYamlDocument.Factory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new UnityYamlDocument.Factory();
        }

        [TestCase("!u!29 &1", false, TestName = "is not GameObject")]
        [TestCase("!u!1 &534047197", true, TestName = "is GameObject")]
        public void IsGameObject(string name, bool isGameObject)
        {
            var doc = _factory.Get(name, "");
            Assert.That(doc.IsGameObject, Is.EqualTo(isGameObject));
        }

        [TestCase("!u!29 &1", true, TestName = "is AnyObject")]
        [TestCase("!u!1 &534047197", false, TestName = "is not AnyObject")]
        public void IsAnyObject(string name, bool IsAnyObject)
        {
            var doc = _factory.Get(name, "");
            Assert.That(doc.IsAnyObject, Is.EqualTo(IsAnyObject));
        }

        [TestCase(null, true, TestName = "is Header")]
        [TestCase("!u!1 &534047197", false, TestName = "is not Header")]
        public void IsHeader(string name, bool IsHeader)
        {
            var doc = _factory.Get(name, "");
            Assert.That(doc.IsHeader, Is.EqualTo(IsHeader));
        }
    }

    [TestCase("!u!29 &1", 1, TestName = "case AnyObject")]
    [TestCase("!u!1 &534047197", 534047197, TestName = "case GameObject")]
    public void ParseFileId(string name, int fileId)
    {
        var doc = _factory.Get(name, "");
        Assert.That(doc.FileId, Is.EqualTo(fileId));
    }
}
