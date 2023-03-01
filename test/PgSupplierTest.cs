using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgSupplierTest : Base
{
    [Test]
    public void RepresentsItselfAsString()
    {
        dynamic supplierFixture = fixtures["suppliers"]["one"];
        var pgSupplier = new PgSupplier(supplierFixture.Id, pgDataSource);
        Assert.AreEqual(supplierFixture.Name, $"{pgSupplier}");
    }
}