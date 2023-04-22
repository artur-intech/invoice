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
        var newEmail = "new email";

        Assert.AreNotEqual(newName, fixture.Name);
        Assert.AreNotEqual(newAddress, fixture.Address);
        Assert.AreNotEqual(newVatNumber, fixture.VatNumber);
        Assert.AreNotEqual(newIban, fixture.Iban);
        Assert.AreNotEqual(newEmail, fixture.Email);

        var pgSupplier = new PgSupplier(fixture.Id, pgDataSource);
        pgSupplier.Modify(newName, newAddress, newVatNumber, newIban, newEmail);

        fixture = SupplierFixture(fixture.Id);
        Assert.AreEqual(newName, fixture.Name);
        Assert.AreEqual(newAddress, fixture.Address);
        Assert.AreEqual(newVatNumber, fixture.VatNumber);
        Assert.AreEqual(newIban, fixture.Iban);
        Assert.AreEqual(newEmail, fixture.Email);
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void DeletesUninvoiced()
    {
        CreateSupplierFixtures();
        dynamic supplierFixture = fixtures["suppliers"]["one"];

        var pgSupplier = new PgSupplier(supplierFixture.Id, pgDataSource);
        pgSupplier.Delete();

        Assert.AreEqual(fixtures["suppliers"].Count - 1, (long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar(), "Supplier should have been deleted.");
    }

    [Test]
    public void CannotBeDeletedWhenHavingInvoices()
    {
        dynamic fixture = fixtures["suppliers"]["one"];

        var exception = Assert.Throws(typeof(Exception), () =>
        {
            var pgSupplier = new PgSupplier(fixture.Id, pgDataSource);
            pgSupplier.Delete();
        });
        StringAssert.Contains("Supplier has invoices and therefore cannot be deleted.", exception.Message);
    }

    [Test]
    public void ReportsName()
    {
        dynamic supplierFixture = fixtures["suppliers"]["one"];
        var pgSupplier = new PgSupplier(supplierFixture.Id, pgDataSource);
        Assert.AreEqual(supplierFixture.Name, pgSupplier.Name());
    }

    [Test]
    public void ReportsDetails()
    {
        dynamic fixture = fixtures["suppliers"]["one"];
        var pgSupplier = new PgSupplier(fixture.Id, pgDataSource);

        pgSupplier.WithDetails((actualId, actualName, actualAddress, actualVatNumber, actualIban, actualEmail) =>
        {
            Assert.AreEqual(fixture.Id, actualId);
            Assert.AreEqual(fixture.Name, actualName);
            Assert.AreEqual(fixture.Address, actualAddress);
            Assert.AreEqual(fixture.VatNumber, actualVatNumber);
            Assert.AreEqual(fixture.Iban, actualIban);
            Assert.AreEqual(fixture.Email, actualEmail);
        });
    }
}
