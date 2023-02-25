using System.Dynamic;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgClientsTest : Base
{
    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void AddsClient()
    {
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM clients").ExecuteScalar());

        var name = "name";
        var address = "address";
        var vatNumber = "vat_number";

        var addedId = new PgClients(pgDataSource).Add(name: name, address: address, vatNumber: vatNumber).Id();

        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM clients").ExecuteScalar());
        dynamic dbRow = DbRow(addedId);
        Assert.AreEqual(name, dbRow.Name);
        Assert.AreEqual(address, dbRow.Address);
        Assert.AreEqual(vatNumber, dbRow.VatNumber);
    }

    [Test]
    public void UniqNameDatabaseConstraint()
    {
        dynamic existingClient = fixtures["clients"]["one"];
        Assert.Throws(typeof(Npgsql.PostgresException), () =>
        {
            new PgClients(pgDataSource).Add(name: existingClient.Name, address: "any", vatNumber: "any");
        });
    }

    ExpandoObject DbRow(int id)
    {
        using var command = pgDataSource.CreateCommand("SELECT * FROM clients WHERE id = $1");
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();
        reader.Read();

        dynamic row = new ExpandoObject();
        row.Name = reader["name"];
        row.Address = reader["address"];
        row.VatNumber = reader["vat_number"];

        return row;
    }
}