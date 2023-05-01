using NUnit.Framework;

namespace Intech.Invoice.Test;

class ConstClientTest
{
    [Test]
    public void ReportsId()
    {
        var id = 1;
        var rawDbData = new Dictionary<string, object>() { { "id", id } };
        var constClient = new ConstClient(new FakeDbDataReader(rawDbData));

        var actual = constClient.Id();

        Assert.AreEqual(id, actual);
    }

    [Test]
    public void ReportsName()
    {
        var name = "name";
        var rawDbData = new Dictionary<string, object>() { { "name", name } };
        var constClient = new ConstClient(new FakeDbDataReader(rawDbData));

        var actual = constClient.Name();

        Assert.AreEqual(name, actual);
    }

    [Test]
    public void RepresentsAsString()
    {
        var name = "name";
        var rawDbData = new Dictionary<string, object>() { { "name", name } };
        var constClient = new ConstClient(new FakeDbDataReader(rawDbData));

        var actual = constClient.Name();

        Assert.AreEqual(name, $"{actual}");
    }

    [Test]
    public void ReportsDetails()
    {
        var id = 1;
        var name = "name";
        var address = "address";
        var vatNumber = "vat";
        var email = "email";
        var rawDbData = new Dictionary<string, object>() { { "id", id }, { "name", name }, { "address", address },
            { "vat_number", vatNumber }, { "email", email } };
        var constClient = new ConstClient(new FakeDbDataReader(rawDbData));

        constClient.WithDetails((actualId, actualName, actualAddress, actualVatNumber, actualEmail) =>
        {
            Assert.AreEqual(id, actualId);
            Assert.AreEqual(name, actualName);
            Assert.AreEqual(address, actualAddress);
            Assert.AreEqual(vatNumber, actualVatNumber);
            Assert.AreEqual(email, actualEmail);
        });
    }
}
