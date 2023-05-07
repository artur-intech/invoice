using NUnit.Framework;

namespace Intech.Invoice.Test;

class ConstInvoiceTest
{
    [Test]
    public void ReportsId()
    {
        var id = 1;
        var rawDbData = new Dictionary<string, object>() { { "id", id } };
        var constInvoice = new ConstInvoice(rawDbData);

        var actual = constInvoice.Id();

        Assert.AreEqual(id, actual);
    }

    [Test]
    public void RepresentsAsString()
    {
        var number = "number";
        var rawDbData = new Dictionary<string, object>() { { "number", number } };
        var constInvoice = new ConstInvoice(rawDbData);

        Assert.AreEqual(number, $"{constInvoice}");
    }

    [Test]
    public void ReportsDetails()
    {
        var id = 1;
        var clientName = "client";
        var number = "number";
        var date = new DateTime(1970, 1, 1);
        var dueDate = new DateTime(1970, 1, 2);
        var subtotal = (long)1;
        var vatAmount = (long)2;
        var total = (long)3;
        var paid = true;
        var paidDate = new DateTime(1970, 1, 3);
        var rawDbData = new Dictionary<string, object>() {
            { "id", id },
            { "client_name", clientName },
            { "number", number },
            { "date", date },
            { "due_date", dueDate },
            { "subtotal", subtotal },
            { "vat_amount", vatAmount },
            { "total", total },
            { "paid", paid },
            { "paid_date", paidDate } };
        var constInvoice = new ConstInvoice(rawDbData);

        constInvoice.WithDetails((actualId, actualClientName, actualNumber, actualDate, actualDueDate, actualSubtotal,
            actualVatAmount, actualTotal, actualPaid, actualPaidDate) =>
        {
            Assert.AreEqual(id, actualId);
            Assert.AreEqual(clientName, actualClientName);
            Assert.AreEqual(number, actualNumber);
            Assert.AreEqual(DateOnly.FromDateTime(date), actualDate);
            Assert.AreEqual(DateOnly.FromDateTime(dueDate), actualDueDate);
            Assert.AreEqual(subtotal, actualSubtotal);
            Assert.AreEqual(vatAmount, actualVatAmount);
            Assert.AreEqual(total, actualTotal);
            Assert.AreEqual(paid, actualPaid);
            Assert.AreEqual(DateOnly.FromDateTime(paidDate), actualPaidDate);
        });
    }
}
