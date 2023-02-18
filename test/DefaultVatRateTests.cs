using NUnit.Framework;

namespace Intech.Invoice.Test;

class DefaultVatRateTests
{
    [Test]
    public void RepresentsItselfAsInteger()
    {
        var stdVatRate = new DefaultVatRate(5);
        Assert.AreEqual(5, stdVatRate.IntValue());
    }

    [Test]
    public void CalculatesVatAmount()
    {
        var stdVatRate = new DefaultVatRate(10);
        Assert.AreEqual(20, stdVatRate.VatAmount(200));
    }
}