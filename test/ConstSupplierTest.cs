using NUnit.Framework;

namespace Intech.Invoice.Test;

class ConstSupplierTest
{
    [Test]
    public void ReportsId()
    {
        var id = 1;
        var rawDbData = new Dictionary<string, object>() { { "id", id } };
        var constSupplier = new ConstSupplier(rawDbData);

        var actual = constSupplier.Id();

        Assert.AreEqual(id, actual);
    }

    [Test]
    public void ReportsName()
    {
        var name = "name";
        var rawDbData = new Dictionary<string, object>() { { "name", name } };
        var constSupplier = new ConstSupplier(rawDbData);

        var actual = constSupplier.Name();

        Assert.AreEqual(name, actual);
    }

    [Test]
    public void RepresentsAsString()
    {
        var name = "name";
        var rawDbData = new Dictionary<string, object>() { { "name", name } };
        var constSupplier = new ConstSupplier(rawDbData);

        var actual = constSupplier.Name();

        Assert.AreEqual(name, $"{actual}");
    }

    [Test]
    public void ReportsDetails()
    {
        var id = 1;
        var name = "name";
        var address = "address";
        var vatNumber = "vat";
        var iban = "iban";
        var email = "email";
        var rawDbData = new Dictionary<string, object>() { { "id", id }, { "name", name }, { "address", address },
            { "vat_number", vatNumber }, { "iban", iban }, { "email", email } };
        var constSupplier = new ConstSupplier(rawDbData);

        constSupplier.WithDetails((actualId, actualName, actualAddress, actualVatNumber, actualIban, actualEmail) =>
        {
            Assert.AreEqual(id, actualId);
            Assert.AreEqual(name, actualName);
            Assert.AreEqual(address, actualAddress);
            Assert.AreEqual(vatNumber, actualVatNumber);
            Assert.AreEqual(iban, actualIban);
            Assert.AreEqual(email, actualEmail);
        });
    }
}
