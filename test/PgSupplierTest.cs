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

    [Test]
    public void ReportsId()
    {
        dynamic supplierFixture = fixtures["suppliers"]["one"];
        var pgSupplier = new PgSupplier(supplierFixture.Id, pgDataSource);
        Assert.AreEqual(supplierFixture.Id, pgSupplier.Id());
    }

    [Test]
    public void ModifiesItself()
    {
        dynamic supplierFixture = fixtures["suppliers"]["one"];
        var newName = "new name";
        var newAddress = "new address";
        var newVatNumber = "new vat";
        var newIban = "new iban";

        Assert.AreNotEqual(newName, supplierFixture.Name);
        Assert.AreNotEqual(newAddress, supplierFixture.Address);
        Assert.AreNotEqual(newVatNumber, supplierFixture.VatNumber);
        Assert.AreNotEqual(newIban, supplierFixture.Iban);

        var pgSupplier = new PgSupplier(supplierFixture.Id, pgDataSource);
        pgSupplier.Modify(newName, newAddress, newVatNumber, newIban);

        supplierFixture = new SupplierFixtures(pgDataSource).Fetch(supplierFixture.Id);
        Assert.AreEqual(newName, supplierFixture.Name);
        Assert.AreEqual(newAddress, supplierFixture.Address);
        Assert.AreEqual(newVatNumber, supplierFixture.VatNumber);
        Assert.AreEqual(newIban, supplierFixture.Iban);
    }

    [Test]
    public void DeletesItself()
    {
        dynamic supplierFixture = fixtures["suppliers"]["one"];

        var pgSupplier = new PgSupplier(supplierFixture.Id, pgDataSource);
        pgSupplier.Delete();

        Assert.AreEqual(fixtures["suppliers"].Count - 1, (long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar(), "Supplier should have been deleted.");
    }

    [Test]
    public void ReportsName()
    {
        dynamic supplierFixture = fixtures["suppliers"]["one"];
        var pgSupplier = new PgSupplier(supplierFixture.Id, pgDataSource);
        Assert.AreEqual(supplierFixture.Name, pgSupplier.Name());
    }
}