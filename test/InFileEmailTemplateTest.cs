using NUnit.Framework;

namespace Intech.Invoice.Test;

class InFileEmailTemplateTest
{
    string? path;

    [Test]
    public void RepresentsAsString()
    {
        var text = "any";
        File.WriteAllText(path, text);

        var template = new InFileEmailTemplate(path);

        Assert.AreEqual(text, $"{template}");
    }

    [SetUp]
    public void CreateFile()
    {
        path = Path.GetTempFileName();
    }

    [TearDown]
    public void DeleteFile()
    {
        File.Delete(path);
    }
}
