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

class InterpolatedEmailBodyTemplateTest : Base
{
    [Test]
    public void Interpolates()
    {
        var text = "{0}, here is your invoice. Total: {1}, due date: {2}";
        var dueDate = ValidDate();
        var total = 100;
        var clientName = "john";

        var template = new InterpolatedEmailTemplate(new EmailTemplate.Fake(text), dueDate, total, clientName);
        var actual = $"{template}";

        Assert.AreEqual($"{clientName}, here is your invoice. Total: {total}, due date: {dueDate}", actual);
    }
}
