using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgInvoiceTest : Base
{
    [Test]
    public void RepresentsItselfAsString()
    {
        dynamic invoice = fixtures["invoices"]["one"];
        var pgInvoice = new PgInvoice(invoice.Id, pgDataSource);
        Assert.AreEqual(invoice.Number, $"{pgInvoice}");
    }

    [Test]
    public void MarksPaid()
    {
        var paidDate = ValidDate();
        dynamic fixture = fixtures["invoices"]["one"];
        Assert.False(fixture.Paid);
        Assert.Null(fixture.PaidDate);
        var pgInvoice = new PgInvoice(fixture.Id, pgDataSource);

        pgInvoice.MarkPaid(paidDate);

        fixture = InvoiceFixture(fixture.Id);
        Assert.True(fixture.Paid, "Invoice should be paid");
        Assert.AreEqual(paidDate, fixture.PaidDate, "Invoice should have given paid date");
    }
}