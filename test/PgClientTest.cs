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
}