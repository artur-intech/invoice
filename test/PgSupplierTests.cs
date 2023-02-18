using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgSupplierTests : TestsBase
{
    [Test]
    public void RepresentsItselfAsString()
    {
        dynamic supplierFixture = fixtures["suppliers"]["one"];
        var pgSupplier = new PgSupplier(id: supplierFixture.Id, pgDataSource: pgDataSource);
        Assert.AreEqual(supplierFixture.Name, $"{pgSupplier}");
    }
}