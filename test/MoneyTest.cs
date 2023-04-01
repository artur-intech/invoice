using NUnit.Framework;

namespace Intech.Invoice.Test;

class MoneyTest : Base
{
    [Test]
    public void RepresentsItselfAsString()
    {
        var hundred = new Money(100);
        Assert.AreEqual("100 EUR", $"{hundred}");
    }
}
