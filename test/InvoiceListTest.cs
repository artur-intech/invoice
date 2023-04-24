using NUnit.Framework;

namespace Intech.Invoice.Test;

class InvoiceListTest : Base
{
    [Test]
    public void Prints()
    {
        var fakeInvoice = new Invoice.Fake();
        var invoiceList = new InvoiceList(new List<Invoice> { fakeInvoice });

        var actual = CapturedStdOut(invoiceList.Print);

        var expected = $"""
            Records total: 1
            {Delimiter()}
            Id: {fakeInvoice.id}
            Client: {fakeInvoice.clientName}
            Number: {fakeInvoice.number}
            Date: {fakeInvoice.date}
            Due date: {fakeInvoice.dueDate}
            Total: {fakeInvoice.total}
            Paid: {fakeInvoice.paid}{Environment.NewLine}
            """;
        Assert.AreEqual(expected, actual);
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
