using System.Dynamic;
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

        var createdInvoiceId = new PgInvoices(pgDataSource).Add(number: number,
                                                       date: date,
                                                       dueDate: dueDate,
                                                       vatRate: vatRate,
                                                       supplierId: supplier.Id,
                                                       clientId: client.Id).Id();

        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM invoices").ExecuteScalar());
        dynamic dbRow = DbRow(createdInvoiceId);
        Assert.AreEqual(number, dbRow.Number);
        Assert.AreEqual(date, dbRow.Date);
        Assert.AreEqual(dueDate, dbRow.DueDate);
        Assert.AreEqual(vatRate, dbRow.VatRate);
        Assert.AreEqual(client.Id, dbRow.ClientId);
        Assert.AreEqual(supplier.Name, dbRow.SupplierName);
        Assert.AreEqual(supplier.Address, dbRow.SupplierAddress);
        Assert.AreEqual(supplier.VatNumber, dbRow.SupplierVatNumber);
        Assert.AreEqual(supplier.Iban, dbRow.SupplierIban);
        Assert.AreEqual(client.Name, dbRow.ClientName);
        Assert.AreEqual(client.Address, dbRow.ClientAddress);
        Assert.AreEqual(client.VatNumber, dbRow.ClientVatNumber);
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
            new PgInvoices(pgDataSource).Add(existingInvoice.Number, ValidDate(), ValidDueDate(), ValidVatRate(), supplierId: ValidSupplierId(), clientId: ValidClientId());
        });
        StringAssert.Contains("violates unique constraint \"uniq_invoice_number\"", exception.Message);
    }

    [Test]
    public void LaterDueDateDbConstraint()
    {
        var pastDueDate = ValidDate().AddDays(-1);

        var exception = Assert.Throws(typeof(Npgsql.PostgresException), () =>
        {
            new PgInvoices(pgDataSource).Add(ValidNumber(), ValidDate(), pastDueDate, ValidVatRate(), supplierId: ValidSupplierId(), clientId: ValidClientId());
        });
        StringAssert.Contains("violates check constraint \"later_invoice_due_date\"", exception.Message);
    }

    [Test]
    public void NonNegativeVatRateDbConstraint()
    {
        var negativeVatRate = -1;

        var exception = Assert.Throws(typeof(Npgsql.PostgresException), () =>
        {
            new PgInvoices(pgDataSource).Add(ValidNumber(), ValidDate(), ValidDueDate(), negativeVatRate, supplierId: ValidSupplierId(), clientId: ValidClientId());
        });
        StringAssert.Contains("violates check constraint \"nonnegative_invoice_vat_rate\"", exception.Message);
    }

    ExpandoObject DbRow(int id)
    {
        using var command = pgDataSource.CreateCommand("SELECT * FROM invoices WHERE id = $1");
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();
        reader.Read();

        dynamic row = new ExpandoObject();
        row.Number = reader["number"];
        row.Date = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date"));
        row.DueDate = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("due_date"));
        row.VatRate = reader["vat_rate"];
        row.ClientId = reader["client_id"];
        row.SupplierName = reader["supplier_name"];
        row.SupplierAddress = reader["supplier_address"];
        row.SupplierVatNumber = reader["supplier_vat_number"];
        row.SupplierIban = reader["supplier_iban"];
        row.ClientName = reader["client_name"];
        row.ClientAddress = reader["client_address"];
        row.ClientVatNumber = reader["client_vat_number"];

        return row;
    }

    string ValidNumber()
    {
        return "1234";
    }

    DateOnly ValidDate()
    {
        return new DateOnly(1970, 01, 01);
    }

    DateOnly ValidDueDate()
    {
        return new DateOnly(1970, 01, 02);
    }

    int ValidVatRate()
    {
        return 20;
    }

    int ValidSupplierId()
    {
        dynamic supplier = fixtures["suppliers"]["one"];
        return supplier.Id;
    }

    int ValidClientId()
    {
        dynamic client = fixtures["clients"]["one"];
        return client.Id;
    }
}