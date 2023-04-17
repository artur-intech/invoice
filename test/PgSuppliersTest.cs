using System.Dynamic;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgSuppliersTest : Base
{
    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void AddsSupplier()
    {
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar());

        var name = ValidName();
        var address = ValidAddress();
        var vatNumber = ValidVatNumber();
        var iban = ValidIban();
        var email = ValidEmail();

        var createdSupplierId = new PgSuppliers(pgDataSource).Add(name, address, vatNumber, iban, email).Id();

        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar());
        dynamic dbRow = DbRow(createdSupplierId);
        Assert.AreEqual(name, dbRow.Name);
        Assert.AreEqual(address, dbRow.Address);
        Assert.AreEqual(vatNumber, dbRow.VatNumber);
        Assert.AreEqual(iban, dbRow.Iban);
    }

    [Test]
    public void UniqNameDbConstraint()
    {
        dynamic existingSupplier = fixtures["suppliers"]["one"];

        var exception = Assert.Throws(typeof(Npgsql.PostgresException), () =>
        {
            new PgSuppliers(pgDataSource).Add(existingSupplier.Name, ValidAddress(), ValidVatNumber(), ValidIban(), ValidEmail());
        });
        StringAssert.Contains("violates unique constraint \"uniq_supplier_name\"", exception.Message);
    }

    ExpandoObject DbRow(int id)
    {
        using var command = pgDataSource.CreateCommand("SELECT * FROM suppliers WHERE id = $1");
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();
        reader.Read();

        dynamic row = new ExpandoObject();
        row.Name = reader["name"];
        row.Address = reader["address"];
        row.VatNumber = reader["vat_number"];
        row.Iban = reader["iban"];

        return row;
    }
}
