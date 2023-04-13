using NUnit.Framework;

namespace Intech.Invoice.Test;

class StrictInputEmailTest
{
    [Test]
    public void ValidatesFormat()
    {
        var invalidValues = new List<string>() { "invalid", "john@" };

        foreach (var invalidValue in invalidValues)
        {
            var exception = Assert.Throws(typeof(Exception), () =>
            {
                new StrictInputEmail(new UserInput.Fake(invalidValue)).ToString();
            });
            Assert.AreEqual("Email has invalid format.", exception.Message);
        }

        Assert.DoesNotThrow(() => { new StrictInputEmail(new UserInput.Fake("a@b.c")).ToString(); });
        Assert.DoesNotThrow(() => { new StrictInputEmail(new UserInput.Fake("john@inbox.test")).ToString(); });
    }
}
