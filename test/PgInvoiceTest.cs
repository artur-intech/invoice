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

    [Test]
    public void CannotBeMarkedPaidRepeatedly()
    {
        dynamic fixture = fixtures["invoices"]["one"];
        pgDataSource.CreateCommand($"UPDATE invoices SET paid = true WHERE id = {fixture.Id}").ExecuteNonQuery();
        var pgInvoice = new PgInvoice(fixture.Id, pgDataSource);

        var exception = Assert.Throws(typeof(Exception), () =>
        {
            pgInvoice.MarkPaid(ValidDate());
        });
        StringAssert.Contains("Cannot mark paid invoice as paid again.", exception.Message);
    }

    [Test]
    public void Nonexistent()
    {
        var nonExistentInvoiceId = 99;
        Assert.Null(pgDataSource.CreateCommand($"SELECT id FROM invoices WHERE id = {nonExistentInvoiceId}").ExecuteScalar());
        var pgInvoice = new PgInvoice(nonExistentInvoiceId, pgDataSource);

        var exception = Assert.Throws(typeof(Exception), () =>
        {
            pgInvoice.MarkPaid(ValidDate());
        });
        StringAssert.Contains("Nonexistent invoice.", exception.Message);
    }
}