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
        dynamic fixture = fixtures["suppliers"]["one"];
        var newName = "new name";
        var newAddress = "new address";
        var newVatNumber = "new vat";
        var newIban = "new iban";

        Assert.AreNotEqual(newName, fixture.Name);
        Assert.AreNotEqual(newAddress, fixture.Address);
        Assert.AreNotEqual(newVatNumber, fixture.VatNumber);
        Assert.AreNotEqual(newIban, fixture.Iban);

        var pgSupplier = new PgSupplier(fixture.Id, pgDataSource);
        pgSupplier.Modify(newName, newAddress, newVatNumber, newIban);

        fixture = SupplierFixture(fixture.Id);
        Assert.AreEqual(newName, fixture.Name);
        Assert.AreEqual(newAddress, fixture.Address);
        Assert.AreEqual(newVatNumber, fixture.VatNumber);
        Assert.AreEqual(newIban, fixture.Iban);
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