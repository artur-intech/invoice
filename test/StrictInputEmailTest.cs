using NUnit.Framework;

namespace Intech.Invoice.Test;

class StrictInputEmailTest
{
    [Test]
    public void ValidatesFormat()
    {
        AssertInvalid("invalid");
        AssertInvalid("a@");

        AssertValid("a@b");
        AssertValid("john@inbox.test");
        AssertValid("JOHN@INBOX.TEST");
    }

    void AssertInvalid(string value)
    {
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            new StrictInputEmail(new UserInput.Fake(value)).ToString();
        }, $"""Value "{value}" should be invalid.""");
        Assert.AreEqual("Email has invalid format.", exception.Message);
    }

    void AssertValid(string value)
    {
        Assert.DoesNotThrow(() => { new StrictInputEmail(new UserInput.Fake(value)).ToString(); }, $"""Value "{value}" should be valid.""");
    }
}
