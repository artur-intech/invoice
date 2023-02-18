using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgClientTests : TestsBase
{
    [Test]
    public void ReturnsName()
    {
        dynamic clientFixture = fixtures["clients"]["one"];
        var pgClient = new PgClient(id: clientFixture.Id, pgDataSource: pgDataSource);
        Assert.AreEqual(clientFixture.Name, $"{pgClient}");
    }
}