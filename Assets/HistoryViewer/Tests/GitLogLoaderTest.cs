using System.Collections.Generic;
using NUnit.Framework;

public class GitLogLoaderTest
{
    [TestCase("a1", "commit1", "a2", "commit2")]
    public void LoadTest(string commit1id, string commit1name, string commit2id, string commit2name)
    {
        var git = new GitCommandExecutorStub(new Dictionary<string, string>
        {
            { "log.* --pretty=\"%H%x09%s\".* -- path", $"{commit1id}\t{commit1name}\n{commit2id}\t{commit2name}\n" }
        });
        var loader = new GitLogLoader(git);
        var commits = loader.Load("path");

        Assert.That(commits.Count, Is.EqualTo(2));
        Assert.That(commits[0].hashId, Is.EqualTo(commit1id));
        Assert.That(commits[0].name, Is.EqualTo(commit1name));
        Assert.That(commits[1].hashId, Is.EqualTo(commit2id));
        Assert.That(commits[1].name, Is.EqualTo(commit2name));
    }
}
