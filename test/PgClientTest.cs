using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgClientTest : Base
{
    [Test]
    public void ReturnsName()
    {
        dynamic clientFixture = fixtures["clients"]["one"];
        var pgClient = new PgClient(clientFixture.Id, pgDataSource);
        Assert.AreEqual(clientFixture.Name, pgClient.Name());
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void DeletesUninvoiced()
    {
        CreateClientFixtures();
        dynamic clientFixture = fixtures["clients"]["one"];

        var pgClient = new PgClient(clientFixture.Id, pgDataSource);
        pgClient.Delete();

        Assert.AreEqual(fixtures["clients"].Count - 1, (long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM clients").ExecuteScalar(), "Client should have been deleted.");
    }

    [Test]
    public void CannotBeDeletedWhenHavingInvoices()
    {
        dynamic clientFixture = fixtures["clients"]["one"];

        var exception = Assert.Throws(typeof(Exception), () =>
        {
            var pgClient = new PgClient(clientFixture.Id, pgDataSource);
            pgClient.Delete();
        });
        StringAssert.Contains("Client has invoices and therefore cannot be deleted.", exception.Message);
    }

    [Test]
    public void RepresentsItselfAsString()
    {
        dynamic clientFixture = fixtures["clients"]["one"];
        var pgClient = new PgClient(clientFixture.Id, pgDataSource);
        Assert.AreEqual(clientFixture.Name, $"{pgClient}");
    }

    [Test]
    public void ModifiesItself()
    {
        dynamic fixture = fixtures["clients"]["one"];
        var newName = "new name";
        var newAddress = "new address";
        var newVatNumber = "new vat";

        Assert.AreNotEqual(newName, fixture.Name);
        Assert.AreNotEqual(newAddress, fixture.Address);
        Assert.AreNotEqual(newVatNumber, fixture.VatNumber);

        var pgClient = new PgClient(fixture.Id, pgDataSource);
        pgClient.Modify(newName, newAddress, newVatNumber);

        fixture = ClientFixture(fixture.Id);
        Assert.AreEqual(newName, fixture.Name);
        Assert.AreEqual(newAddress, fixture.Address);
        Assert.AreEqual(newVatNumber, fixture.VatNumber);
    }

    [Test]
    public void ReportsId()
    {
        dynamic clientFixture = fixtures["clients"]["one"];
        var pgClient = new PgSupplier(clientFixture.Id, pgDataSource);
        Assert.AreEqual(clientFixture.Id, pgClient.Id());
    }

    [Test]
    public void ReportsDetails()
    {
        dynamic fixture = fixtures["clients"]["one"];
        var pgClient = new PgClient(fixture.Id, pgDataSource);

        pgClient.WithDetails((actualId, actualName, actualAddress, actualVatNumber, actualEmail) =>
        {
            Assert.AreEqual(fixture.Id, actualId);
            Assert.AreEqual(fixture.Name, actualName);
            Assert.AreEqual(fixture.Address, actualAddress);
            Assert.AreEqual(fixture.VatNumber, actualVatNumber);
            Assert.AreEqual(fixture.Email, actualEmail);
        });
    }
}
