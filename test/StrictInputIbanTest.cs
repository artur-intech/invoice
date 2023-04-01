using NUnit.Framework;

namespace Intech.Invoice.Test;

class StrictInputIbanTest
{
    [Test]
    [TestCase("invalid")]
    [TestCase("d12a")]
    [TestCase("de1")]
    [TestCase("de12")]
    public void ValidatesFormat(string invalidInput)
    {
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            new StrictInputIban(new UserInput.Fake(invalidInput)).ToString();
        });
        Assert.AreEqual("IBAN has invalid format.", exception.Message);

        Assert.DoesNotThrow(() => { new StrictInputIban(new UserInput.Fake("de12a")).ToString(); });
        Assert.DoesNotThrow(() => { new StrictInputIban(new UserInput.Fake("DE12a")).ToString(); });
        Assert.DoesNotThrow(() => { new StrictInputIban(new UserInput.Fake("de121")).ToString(); });
    }
}
