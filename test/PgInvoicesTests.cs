using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgInvoicesTests : TestsBase
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
        var number = "1234";
        var date = new DateOnly(1970, 01, 01);
        var dueDate = new DateOnly(1970, 01, 02);
        var vatRate = 20;

        var addedId = new PgInvoices(pgDataSource).Add(number: number,
                                                       date: date,
                                                       dueDate: dueDate,
                                                       vatRate: vatRate,
                                                       supplierId: supplier.Id,
                                                       clientId: client.Id).Id();

        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM invoices").ExecuteScalar());
        var sql = "SELECT * FROM invoices WHERE id = $1";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(addedId);
        using var reader = command.ExecuteReader();
        reader.Read();
        Assert.AreEqual(number, reader["number"]);
        Assert.AreEqual(date, reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date")));
        Assert.AreEqual(dueDate, reader.GetFieldValue<DateOnly>(reader.GetOrdinal("due_date")));
        Assert.AreEqual(vatRate, reader["vat_rate"]);
        Assert.AreEqual(client.Id, reader["client_id"]);
        Assert.AreEqual(supplier.Name, reader["supplier_name"]);
        Assert.AreEqual(supplier.Address, reader["supplier_address"]);
        Assert.AreEqual(supplier.VatNumber, reader["supplier_vat_number"]);
        Assert.AreEqual(supplier.Iban, reader["supplier_iban"]);
        Assert.AreEqual(client.Name, reader["client_name"]);
        Assert.AreEqual(client.Address, reader["client_address"]);
        Assert.AreEqual(client.VatNumber, reader["client_vat_number"]);
    }
}