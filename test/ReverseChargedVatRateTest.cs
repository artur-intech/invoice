using NUnit.Framework;

namespace Intech.Invoice.Test;

public class ReverseChargedVatRateTest
{
    [Test]
    public void RepresentsItselfAsInteger()
    {
        var reverseChargedVatRate = new ReverseChargedVatRate();
        Assert.Zero(reverseChargedVatRate.IntValue());
    }

    [Test]
    public void ReturnsZeroVatAmount()
    {
        var reverseChargedVatRate = new ReverseChargedVatRate();
        Assert.Zero(reverseChargedVatRate.VatAmount(100));
    }
}