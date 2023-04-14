using NUnit.Framework;

namespace Intech.Invoice.Test;

class StrictInputIbanTest
{
    // https://en.wikipedia.org/wiki/International_Bank_Account_Number#Structure
    [Test]
    public void ValidatesFormat()
    {
        AssertInvalid("invalid");
        AssertInvalid("d12a");
        AssertInvalid("de1");
        AssertInvalid("de12");
        AssertInvalid("de12" + new string('1', 31));

        AssertValid("de12a");
        AssertValid("DE12a");
        AssertValid("de121");
        AssertValid("de12" + new string('1', 30));
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
