using System.Collections.Generic;
using NUnit.Framework;
using Negi0109.HistoryViewer.Models;
using System.IO;
using System.Text;

public class UnityYamlDocumentParserTest
{
    [TestCase("!u!29 &1", false, TestName = "is not GameObject")]
    [TestCase("!u!1 &534047197", true, TestName = "is GameObject")]
    public void Parse(string name, bool isGameObject)
    {
        var doc = new UnityYamlDocument(name, "");
        Assert.That(doc.IsGameObject, Is.EqualTo(isGameObject));
    }
}
