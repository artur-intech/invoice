using NUnit.Framework;

namespace Intech.Invoice.Test;

class StrictInputVatNumberTest
{
    // https://en.wikipedia.org/wiki/VAT_identification_number
    [Test]
    public void ValidatesFormat()
    {
        AssertInvalid("D12");
        AssertInvalid("DE1");
        AssertInvalid("DE_more_than_max");

        AssertValid("de12");
        AssertValid("DE12");
    }

    void AssertInvalid(string value)
    {
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            new StrictInputVatNumber(new UserInput.Fake(value)).ToString();
        }, $"""Value "{value}" should be invalid.""");
        Assert.AreEqual("VAT number has invalid format.", exception.Message);
    }

    void AssertValid(string value)
    {
        Assert.DoesNotThrow(() => { new StrictInputVatNumber(new UserInput.Fake(value)).ToString(); }, $"""Value "{value}" should be valid.""");
    }
}
