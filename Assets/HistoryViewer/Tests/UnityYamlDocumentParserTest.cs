using System.Collections.Generic;
using NUnit.Framework;
using Negi0109.HistoryViewer.Models;
using System.IO;
using System.Text;

public class UnityYamlDocumentParserTest
{
    public class ParseType
    {
        [TestCase("!u!29 &1", false, TestName = "is not GameObject")]
        [TestCase("!u!1 &534047197", true, TestName = "is GameObject")]
        public void IsGameObject(string name, bool isGameObject)
        {
            var doc = new UnityYamlDocument(name, "");
            Assert.That(doc.IsGameObject, Is.EqualTo(isGameObject));
        }

        [TestCase("!u!29 &1", true, TestName = "is AnyObject")]
        [TestCase("!u!1 &534047197", false, TestName = "is not AnyObject")]
        public void IsAnyObject(string name, bool IsAnyObject)
        {
            var doc = new UnityYamlDocument(name, "");
            Assert.That(doc.IsAnyObject, Is.EqualTo(IsAnyObject));
        }

        [TestCase(null, true, TestName = "is Header")]
        [TestCase("!u!1 &534047197", false, TestName = "is not Header")]
        public void IsHeader(string name, bool IsHeader)
        {
            var doc = new UnityYamlDocument(name, "");
            Assert.That(doc.IsHeader, Is.EqualTo(IsHeader));
        }
    }
}
