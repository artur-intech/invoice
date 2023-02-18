using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgInvoiceTests : TestsBase
{
    [Test]
    public void RepresentsItselfAsString()
    {
        dynamic invoice = fixtures["invoices"]["one"];
        var pgInvoice = new PgInvoice(invoice.Id, pgDataSource);
        Assert.AreEqual(invoice.Number, $"{pgInvoice}");
    }
}