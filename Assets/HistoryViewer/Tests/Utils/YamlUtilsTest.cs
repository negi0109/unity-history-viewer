using NUnit.Framework;
using Negi0109.HistoryViewer.Models;

public class YamlUtilsTest
{
    [TestCase("   m_Name: Main Camera 2", "m_Name: ", "Main Camera 2")]
    [TestCase("   - component: {fileID: 534047200}", "- component: ", "{fileID: 534047200}")]
    public void GetInlineValue(string text, string key, string ret)
    {
        Assert.That(YamlUtils.GetInlineValue(text, key), Is.EqualTo(ret));
    }

    [TestCase("{a1: b1}", "a1", "b1")]
    [TestCase("{a1: b1, a2: b2}", "a1", "b1")]
    [TestCase("{a1: b1, a2: b2}", "a2", "b2")]
    [TestCase("{a1: b1, a2: b2, a3: b3}", "a2", "b2")]
    [TestCase("{a1: b1, a2: b2, a3: b3}", "a2", "b2")]
    [TestCase("{a1: b1, a2: b2, a3: b3}", "a3", "b3")]
    public void GetBlockValue(string text, string key, string ret)
    {
        Assert.That(YamlUtils.GetBlockValue(text, key), Is.EqualTo(ret));
    }

    [TestCase("--- !u!1 &1", "!u!1 &1")]
    public void GetDocumentName(string text, string ret)
    {
        Assert.That(YamlUtils.GetDocumentName(text), Is.EqualTo(ret));
    }

    [TestCase("GameObject:", false)]
    [TestCase("--- !u!1 &1", true)]
    public void IsDocumentDelimiter(string text, bool ret)
    {
        Assert.That(YamlUtils.IsDocumentDelimiter(text), Is.EqualTo(ret));
    }
}
