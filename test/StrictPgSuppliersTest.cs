using NUnit.Framework;

namespace Intech.Invoice.Test;

class StrictPgSuppliersTest : Base
{
    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void EnsuresNonemptySupplierName(string name)
    {
        var strictPgSuppliers = new StrictPgSuppliers(new Suppliers.Fake());
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            strictPgSuppliers.Add(name: name, address: ValidAddress(), vatNumber: ValidVatNumber(), iban: ValidIban());
        });
        Assert.AreEqual("Supplier name cannot be empty.", exception.Message);
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void EnsuresNonemptySupplierAddress(string address)
    {
        var strictPgSuppliers = new StrictPgSuppliers(new Suppliers.Fake());
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            strictPgSuppliers.Add(name: "any", address: address, vatNumber: ValidVatNumber(), iban: ValidIban());
        });
        Assert.AreEqual("Supplier address cannot be empty.", exception.Message);
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void EnsuresNonemptySupplierIban(string iban)
    {
        var strictPgSuppliers = new StrictPgSuppliers(new Suppliers.Fake());
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            strictPgSuppliers.Add(name: "any", address: ValidAddress(), vatNumber: ValidVatNumber(), iban: iban);
        });
        Assert.AreEqual("Supplier IBAN cannot be empty.", exception.Message);
    }
}