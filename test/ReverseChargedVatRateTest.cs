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

    [Test]
    public void Equality()
    {
        var one = new ReverseChargedVatRate();
        var other = new ReverseChargedVatRate();

        Assert.True(one.Equals(one));
        Assert.True(one.Equals(other));
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(one == one);
        Assert.True(one == other);
        Assert.False(one != other);
#pragma warning restore CS1718 // Comparison made to same variable

        Assert.AreEqual(HashCode.Combine(one.IntValue()), one.GetHashCode());
    }
}
