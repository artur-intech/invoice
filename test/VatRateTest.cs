using NUnit.Framework;

namespace Intech.Invoice.Test;

class VatRateTest
{
    [Test]
    public void CreatesStandard()
    {
        const int rate = 20;
        var standardVatRate = VatRate.Standard();
        // TODO Implement VatRate identity
        Assert.AreEqual(rate, standardVatRate.IntValue());
    }
}