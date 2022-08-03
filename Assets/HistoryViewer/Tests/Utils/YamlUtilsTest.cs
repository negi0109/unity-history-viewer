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

    [TestCase("   a: 1", "a", true)]
    [TestCase("   a: {b: 1, a: 1}", "a", true)]
    [TestCase("   a: {b: 1, a: 1}", "b", false)]
    public void IsKey(string text, string key, bool ret)
    {
        Assert.That(YamlUtils.IsKey(text, key), Is.EqualTo(ret));
    }

    [TestCase("- a: 1", true)]
    [TestCase("a: 1", false)]
    [TestCase("  - a: 1", true)]
    [TestCase("  a: 1", false)]
    [TestCase("- a: {b: 1, c: 1}", true)]
    [TestCase("a: {b: 1, c: 1}", false)]
    [TestCase("  - a: {b: 1, c: 1}", true)]
    [TestCase("    a: {b: 1, c: 1}", false)]
    public void IsArrayElement(string text, bool ret)
    {
        Assert.That(YamlUtils.IsArrayElement(text), Is.EqualTo(ret));
    }

    [TestCase("  a1: {b: 1, c: 1}", "a1")]
    [TestCase("a1: {b: 1, c: 1}", "a1")]
    [TestCase("a1", null)]
    public void GetInlineKey(string text, string key)
    {
        Assert.That(YamlUtils.GetInlineKey(text), Is.EqualTo(key));
    }

    [TestCase("  a1: {b: 1, c: 1}", 2)]
    [TestCase("      a1: {b: 1, c: 1}", 6)]
    [TestCase("a1: {b: 1, c: 1}", 0)]
    [TestCase("  ", 2)]
    public void GetIndentSize(string text, int size)
    {
        Assert.That(YamlUtils.GetIndentSize(text), Is.EqualTo(size));
    }
}
