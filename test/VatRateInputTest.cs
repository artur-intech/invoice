using NUnit.Framework;

namespace Intech.Invoice.Test;

class VatRateInputTest
{
    [Test]
    public void ReturnsReverseChargedVatRate()
    {
        var vatRateInput = new VatRateInput("reverse-charged");
        Assert.AreEqual(new ReverseChargedVatRate().IntValue(), vatRateInput.VatRate().IntValue());
    }

    [Test]
    public void ReturnsDefaultVatRate()
    {
        var vatRateInput = new VatRateInput("20");
        Assert.AreEqual(new DefaultVatRate(20).IntValue(), vatRateInput.VatRate().IntValue());
    }

    [Test]
    [TestCase("invalid", "VAT rate must be a number")]
    [TestCase("-1", "VAT rate must be positive")]
    public void ProhibitsInvalidValue(string value, string errorMessage)
    {
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            new VatRateInput(value).VatRate();
        });
        Assert.AreEqual(errorMessage, exception.Message);
    }
}