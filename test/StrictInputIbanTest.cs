using NUnit.Framework;

namespace Intech.Invoice.Test;

class StrictInputIbanTest
{
    [Test]
    public void ValidatesFormat()
    {
        AssertInvalid("invalid");
        AssertInvalid("d12a");
        AssertInvalid("de1");
        AssertInvalid("de12");

        AssertValid("de12a");
        AssertValid("DE12a");
        AssertValid("de121");
    }

    void AssertInvalid(string value)
    {
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            new StrictInputIban(new UserInput.Fake(value)).ToString();
        }, $"""Value "{value}" should be invalid.""");
        Assert.AreEqual("IBAN has invalid format.", exception.Message);
    }

    void AssertValid(string value)
    {
        Assert.DoesNotThrow(() => { new StrictInputIban(new UserInput.Fake(value)).ToString(); }, $"""Value "{value}" should be valid.""");
    }
}
