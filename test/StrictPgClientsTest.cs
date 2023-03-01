using NUnit.Framework;

namespace Intech.Invoice.Test;

class StrictPgClientsTest : Base
{
    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void EnsuresNonemptyClientName(string name)
    {
        var strictPgClients = new StrictPgClients(new Clients.Fake());
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            strictPgClients.Add(name: name, address: ValidAddress(), vatNumber: ValidVatNumber());
        });
        Assert.AreEqual("Client name cannot be empty.", exception.Message);
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void EnsuresNonemptyClientAddress(string address)
    {
        var strictPgClients = new StrictPgClients(new Clients.Fake());
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            strictPgClients.Add(name: "any", address: address, vatNumber: ValidVatNumber());
        });
        Assert.AreEqual("Client address cannot be empty.", exception.Message);
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void EnsuresNonemptyClientVatNumber(string vatNumber)
    {
        var strictPgClients = new StrictPgClients(new Clients.Fake());
        var exception = Assert.Throws(typeof(Exception), () =>
        {
            strictPgClients.Add(name: "any", address: ValidAddress(), vatNumber: vatNumber);
        });
        Assert.AreEqual("Client VAT number cannot be empty.", exception.Message);
    }
}