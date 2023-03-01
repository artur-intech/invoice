using NUnit.Framework;

namespace Intech.Invoice.Test;

class UniqPgClientsTest : Base
{
    [Test]
    public void ValidatesClientNameUniqueness()
    {
        dynamic existingClient = fixtures["clients"]["one"];

        var uniqPgClients = new UniqPgClients(new Clients.Fake(), pgDataSource);
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            uniqPgClients.Add(name: existingClient.Name, address: ValidAddress(), vatNumber: ValidVatNumber());
        });
        Assert.AreEqual("Client name has already been taken.", exception.Message);
    }
}