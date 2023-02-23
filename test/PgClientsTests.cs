using NUnit.Framework;

namespace Intech.Invoice.Test;

class PgClientsTests : TestsBase
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
        var sql = "SELECT * FROM clients WHERE id = $1";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(addedId);
        using var reader = command.ExecuteReader();
        reader.Read();
        Assert.AreEqual(name, reader["name"]);
        Assert.AreEqual(address, reader["address"]);
        Assert.AreEqual(vatNumber, reader["vat_number"]);
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
}