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
        dynamic clientFixture = fixtures["clients"]["one"];
        var newName = "new name";
        var newAddress = "new address";
        var newVatNumber = "new vat";

        Assert.AreNotEqual(newName, clientFixture.Name);
        Assert.AreNotEqual(newAddress, clientFixture.Address);
        Assert.AreNotEqual(newVatNumber, clientFixture.VatNumber);

        var pgClient = new PgClient(clientFixture.Id, pgDataSource);
        pgClient.Modify(newName, newAddress, newVatNumber);

        clientFixture = new ClientFixtures(pgDataSource).Fetch(clientFixture.Id);
        Assert.AreEqual(newName, clientFixture.Name);
        Assert.AreEqual(newAddress, clientFixture.Address);
        Assert.AreEqual(newVatNumber, clientFixture.VatNumber);
    }

    [Test]
    public void ReportsId()
    {
        dynamic clientFixture = fixtures["clients"]["one"];
        var pgClient = new PgSupplier(clientFixture.Id, pgDataSource);
        Assert.AreEqual(clientFixture.Id, pgClient.Id());
    }
}