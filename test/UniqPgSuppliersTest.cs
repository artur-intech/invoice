using NUnit.Framework;

namespace Intech.Invoice.Test;

class UniqPgSuppliersTest : Base
{
    [Test]
    public void ValidatesSupplierNameUniqueness()
    {
        dynamic existingSupplier = fixtures["suppliers"]["one"];

        var uniqPgSuppliers = new UniqPgSuppliers(new Suppliers.Fake(), pgDataSource);
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            uniqPgSuppliers.Add(existingSupplier.Name, ValidAddress(), ValidVatNumber(), ValidIban());
        });
        Assert.AreEqual("Supplier name has already been taken.", exception.Message);
    }
}
