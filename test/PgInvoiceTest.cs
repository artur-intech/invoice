using MimeKit;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgInvoiceTest : Base
{
    [Test]
    public void ReportsId()
    {
        var id = 1;
        var pgInvoice = new PgInvoice(id, pgDataSource);
        Assert.AreEqual(id, pgInvoice.Id());
    }

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

    [Test]
    public void Sends()
    {
        dynamic fixture = fixtures["invoices"]["one"];
        dynamic clientFixture = fixtures["clients"]["one"];
        dynamic supplierFixture = fixtures["suppliers"]["one"];
        var pgInvoice = new PgInvoice(fixture.Id, pgDataSource);
        var smtpClient = new FakeSmtpClient();

        pgInvoice.Send(smtpClient);

        var sentEmail = smtpClient.Deliveries().First();
        var expectedBody = new InterpolatedEmailTemplate(new InFileEmailTemplate("assets/email_template.txt"), fixture.DueDate, fixture.Total, clientFixture.Name, supplierFixture.Name).ToString();
        Assert.AreEqual(InternetAddressList.Parse($"{supplierFixture.Name} <{supplierFixture.Email}>"), sentEmail.From);
        Assert.AreEqual(InternetAddressList.Parse(clientFixture.Email), sentEmail.To);
        Assert.AreEqual($"Invoice no. {fixture.Number}", sentEmail.Subject);
        Assert.AreEqual(expectedBody, sentEmail.TextBody);
        Assert.AreEqual(ContentType.Parse($"application/pdf; name=invoice_{fixture.Number}_{fixture.SupplierName}.pdf").ToString(),
        sentEmail.Attachments.First().ContentType.ToString());
        Assert.True(sentEmail.Attachments.First().IsAttachment);
    }

    [Test]
    public void ReportsDetails()
    {
        dynamic fixture = fixtures["invoices"]["one"];
        var pgInvoice = new PgInvoice(fixture.Id, pgDataSource);

        pgInvoice.WithDetails((actualId, actualClientName, actualNumber, actualDate, actualDueDate, actualSubtotal,
            actualVatAmount, actualTotal, actualPaid, actualPaidDate) =>
        {
            Assert.AreEqual(fixture.Id, actualId);
            Assert.AreEqual(fixture.ClientName, actualClientName);
            Assert.AreEqual(fixture.Number, actualNumber);
            Assert.AreEqual(fixture.Date, actualDate);
            Assert.AreEqual(fixture.DueDate, actualDueDate);
            Assert.AreEqual(fixture.Subtotal, actualSubtotal);
            Assert.AreEqual(fixture.VatAmount, actualVatAmount);
            Assert.AreEqual(fixture.Total, actualTotal);
            Assert.AreEqual(fixture.Paid, actualPaid);
            Assert.AreEqual(fixture.PaidDate, actualPaidDate);
        });
    }
}
