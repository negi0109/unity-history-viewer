using NUnit.Framework;
using static Negi0109.HistoryViewer.Models.YamlUtils;

public class YamlUtilsTest
{

    [TestCase("   m_Name: Main Camera 2", "m_Name: ", "Main Camera 2")]
    [TestCase("   - component: {fileID: 534047200}", "- component: ", "{fileID: 534047200}")]
    public void GetInlineValueTest(string text, string key, string ret)
    {
        Assert.That(GetInlineValue(text, key), Is.EqualTo(ret));
        Assert.That(GetInlineValue(text, key, GetIndentSize(text)), Is.EqualTo(ret));
    }

    [TestCase("{a1: b1}", "a1", "b1")]
    [TestCase("{a1: b1, a2: b2}", "a1", "b1")]
    [TestCase("{a1: b1, a2: b2}", "a2", "b2")]
    [TestCase("{a1: b1, a2: b2, a3: b3}", "a2", "b2")]
    [TestCase("{a1: b1, a2: b2, a3: b3}", "a2", "b2")]
    [TestCase("{a1: b1, a2: b2, a3: b3}", "a3", "b3")]
    public void GetBlockValueTest(string text, string key, string ret)
    {
        Assert.That(GetBlockValue(text, key), Is.EqualTo(ret));
    }

    [TestCase("--- !u!1 &1", "!u!1 &1")]
    public void GetDocumentNameTest(string text, string ret)
    {
        Assert.That(GetDocumentName(text), Is.EqualTo(ret));
    }

    [TestCase("GameObject:", false)]
    [TestCase("--- !u!1 &1", true)]
    public void IsDocumentDelimiterTest(string text, bool ret)
    {
        Assert.That(IsDocumentDelimiter(text), Is.EqualTo(ret));
    }

    [TestCase("   a: 1", "a", true)]
    [TestCase("   a: {b: 1, a: 1}", "a", true)]
    [TestCase("   a: {b: 1, a: 1}", "b", false)]
    public void IsKeyTest(string text, string key, bool ret)
    {
        Assert.That(IsKey(text, key), Is.EqualTo(ret));
        Assert.That(IsKey(text, key, GetIndentSize(text)), Is.EqualTo(ret));
    }

    [TestCase("- a: 1", true)]
    [TestCase("a: 1", false)]
    [TestCase("  - a: 1", true)]
    [TestCase("  a: 1", false)]
    [TestCase("- a: {b: 1, c: 1}", true)]
    [TestCase("a: {b: 1, c: 1}", false)]
    [TestCase("  - a: {b: 1, c: 1}", true)]
    [TestCase("    a: {b: 1, c: 1}", false)]
    public void IsArrayElementTest(string text, bool ret)
    {
        Assert.That(IsArrayElement(text), Is.EqualTo(ret));
        Assert.That(IsArrayElement(text, GetIndentSize(text)), Is.EqualTo(ret));
    }

    [TestCase("  a1: {b: 1, c: 1}", "a1")]
    [TestCase("a1: {b: 1, c: 1}", "a1")]
    [TestCase("a1", null)]
    public void GetInlineKeyTest(string text, string key)
    {
        Assert.That(GetInlineKey(text), Is.EqualTo(key));
        Assert.That(GetInlineKey(text, GetIndentSize(text)), Is.EqualTo(key));
    }

    [TestCase("  a1: {b: 1, c: 1}", 2)]
    [TestCase("      a1: {b: 1, c: 1}", 6)]
    [TestCase("a1: {b: 1, c: 1}", 0)]
    [TestCase("  ", 2)]
    public void GetIndentSizeTest(string text, int size)
    {
        Assert.That(GetIndentSize(text), Is.EqualTo(size));
    }
}
