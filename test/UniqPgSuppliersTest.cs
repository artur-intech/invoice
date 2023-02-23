using NUnit.Framework;

namespace Intech.Invoice.Test;

class UniqPgSuppliersTests : TestsBase
{
    [Test]
    public void ValidatesSupplierNameUniqueness()
    {
        dynamic existingSupplier = fixtures["suppliers"]["one"];

        var uniqPgSuppliers = new UniqPgSuppliers(new Suppliers.Fake(), pgDataSource);
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            uniqPgSuppliers.Add(name: existingSupplier.Name, address: "any", vatNumber: "any", iban: "any");
        });
        Assert.AreEqual("Supplier name has already been taken.", exception.Message);
    }
}