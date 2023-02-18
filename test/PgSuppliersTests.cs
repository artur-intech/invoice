using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgSuppliersTests : TestsBase
{
    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void AddsSupplier()
    {
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar());

        var name = "name";
        var address = "address";
        var vatNumber = "vat_number";
        var iban = "iban";

        var addedId = new PgSuppliers(pgDataSource).Add(name: name, address: address, vatNumber: vatNumber, iban: iban).Id();

        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar());
        var sql = "SELECT * FROM suppliers WHERE id = $1";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(addedId);
        using var reader = command.ExecuteReader();
        reader.Read();
        Assert.AreEqual(name, reader["name"]);
        Assert.AreEqual(address, reader["address"]);
        Assert.AreEqual(vatNumber, reader["vat_number"]);
        Assert.AreEqual(iban, reader["iban"]);
    }
}