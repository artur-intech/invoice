using NUnit.Framework;

namespace Intech.Invoice.Test;

class DefaultVatRateTest
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

    [Test]
    public void Equality()
    {
        var ten = new DefaultVatRate(10);
        var twenty = new DefaultVatRate(20);

        Assert.True(ten.Equals(ten));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(ten == ten);
#pragma warning restore CS1718 // Comparison made to same variable

        Assert.False(ten.Equals(twenty));
        Assert.False(ten == twenty);
        Assert.True(ten != twenty);

        Assert.AreEqual(HashCode.Combine(ten.IntValue()), ten.GetHashCode());
    }
}