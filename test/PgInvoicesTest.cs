using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgInvoicesTest : Base
{
    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void AddsInvoice()
    {
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM invoices").ExecuteScalar());

        CreateSupplierFixtures();
        CreateClientFixtures();
        dynamic supplier = fixtures["suppliers"]["one"];
        dynamic client = fixtures["clients"]["one"];
        var number = ValidNumber();
        var date = ValidDate();
        var dueDate = ValidDueDate();
        var vatRate = ValidVatRate();

        var createdInvoiceId = new PgInvoices(pgDataSource).Add(number, date, dueDate, vatRate, supplier.Id, client.Id).Id();

        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM invoices").ExecuteScalar());
        dynamic fixture = InvoiceFixture(createdInvoiceId);
        Assert.AreEqual(number, fixture.Number);
        Assert.AreEqual(date, fixture.Date);
        Assert.AreEqual(dueDate, fixture.DueDate);
        Assert.AreEqual(vatRate, fixture.VatRate);
        Assert.AreEqual(client.Id, fixture.ClientId);
        Assert.AreEqual(supplier.Name, fixture.SupplierName);
        Assert.AreEqual(supplier.Address, fixture.SupplierAddress);
        Assert.AreEqual(supplier.VatNumber, fixture.SupplierVatNumber);
        Assert.AreEqual(supplier.Iban, fixture.SupplierIban);
        Assert.AreEqual(client.Name, fixture.ClientName);
        Assert.AreEqual(client.Address, fixture.ClientAddress);
        Assert.AreEqual(client.VatNumber, fixture.ClientVatNumber);
        Assert.False(fixture.Paid);
    }

    [Test]
    public void ThrowsExceptionWhenSupplierDoesNotExist()
    {
        var nonexistentSupplierId = 99;

        var exception = Assert.Throws(typeof(Exception), () =>
            {
                new PgInvoices(pgDataSource).Add(number: ValidNumber(),
                    date: ValidDate(),
                    dueDate: ValidDueDate(),
                    vatRate: ValidVatRate(),
                    supplierId: nonexistentSupplierId,
                    clientId: ValidClientId());
            });
        Assert.AreEqual("Database query didn't return invoice id.", exception.Message);
    }

    [Test]
    public void UniqNumberDbConstraint()
    {
        dynamic existingInvoice = fixtures["invoices"]["one"];

        var exception = Assert.Throws(typeof(Npgsql.PostgresException), () =>
        {
            new PgInvoices(pgDataSource).Add(existingInvoice.Number, ValidDate(), ValidDueDate(), ValidVatRate(), ValidSupplierId(), ValidClientId());
        });
        StringAssert.Contains("violates unique constraint \"uniq_invoice_number\"", exception.Message);
    }

    [Test]
    public void LaterDueDateDbConstraint()
    {
        var pastDueDate = ValidDate().AddDays(-1);

        var exception = Assert.Throws(typeof(Npgsql.PostgresException), () =>
        {
            new PgInvoices(pgDataSource).Add(ValidNumber(), ValidDate(), pastDueDate, ValidVatRate(), ValidSupplierId(), ValidClientId());
        });
        StringAssert.Contains("violates check constraint \"later_invoice_due_date\"", exception.Message);
    }

    [Test]
    public void NonNegativeVatRateDbConstraint()
    {
        var negativeVatRate = -1;

        var exception = Assert.Throws(typeof(Npgsql.PostgresException), () =>
        {
            new PgInvoices(pgDataSource).Add(ValidNumber(), ValidDate(), ValidDueDate(), negativeVatRate, ValidSupplierId(), ValidClientId());
        });
        StringAssert.Contains("violates check constraint \"nonnegative_invoice_vat_rate\"", exception.Message);
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void SortsChronologically()
    {
        CreateSupplierFixtures();
        CreateClientFixtures();
        dynamic thirdInvoiceFixture = CreateInvoiceFixture(new DateOnly(1970, 1, 1));
        dynamic firstInvoiceFixture = CreateInvoiceFixture(new DateOnly(1970, 1, 3));
        dynamic secondInvoiceFixture = CreateInvoiceFixture(new DateOnly(1970, 1, 2));
        CreateLineItemFixture(thirdInvoiceFixture.Id);
        CreateLineItemFixture(firstInvoiceFixture.Id);
        CreateLineItemFixture(secondInvoiceFixture.Id);

        var pgInvoices = new PgInvoices(pgDataSource).ToList();

        CollectionAssert.AreEqual(new List<int> { firstInvoiceFixture.Id, secondInvoiceFixture.Id, thirdInvoiceFixture.Id },
            pgInvoices.Select(i => i.Id()).ToList(),
            "Invoices should be sorted by date in the descending order.");
    }

    DateOnly ValidDueDate()
    {
        return ValidDate();
    }
}