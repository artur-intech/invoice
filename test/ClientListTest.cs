using NUnit.Framework;

namespace Intech.Invoice.Test;

class ClientListTest : Base
{
    [Test]
    public void Prints()
    {
        var fakeClient = new Client.Fake();
        var clientList = new ClientList(new List<Client> { fakeClient });

        var actual = CapturedStdOut(clientList.Print);

        var expected = $"""
            Records total: 1
            {Delimiter()}
            Id: {fakeClient.id}
            Name: {fakeClient.name}
            Address: {fakeClient.address}
            VAT number: {fakeClient.vatNumber}
            Email: {fakeClient.email}{Environment.NewLine}
            """;
        Assert.AreEqual(expected, actual);
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
