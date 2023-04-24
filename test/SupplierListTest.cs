using NUnit.Framework;

namespace Intech.Invoice.Test;

class SupplierListTest : Base
{
    [Test]
    public void Prints()
    {
        var fakeSupplier = new Supplier.Fake();
        var supplierList = new SupplierList(new List<Supplier> { fakeSupplier });

        var actual = CapturedStdOut(supplierList.Print);

        var expected = $"""
            Records total: 1
            {Delimiter()}
            Id: {fakeSupplier.id}
            Name: {fakeSupplier.name}
            Address: {fakeSupplier.address}
            VAT number: {fakeSupplier.vatNumber}
            IBAN: {fakeSupplier.iban}
            Email: {fakeSupplier.email}{Environment.NewLine}
            """;
        Assert.AreEqual(expected, actual);
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
