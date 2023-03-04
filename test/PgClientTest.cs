using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgClientTest : Base
{
    [Test]
    public void ReturnsName()
    {
        dynamic clientFixture = fixtures["clients"]["one"];
        var pgClient = new PgClient(clientFixture.Id, pgDataSource);
        Assert.AreEqual(clientFixture.Name, $"{pgClient}");
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
}