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

        var name = ValidName();
        var address = ValidAddress();
        var vatNumber = ValidVatNumber();

        var createdClientId = new PgClients(pgDataSource).Add(name, address, vatNumber).Id();

        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM clients").ExecuteScalar());
        dynamic dbRow = DbRow(createdClientId);
        Assert.AreEqual(name, dbRow.Name);
        Assert.AreEqual(address, dbRow.Address);
        Assert.AreEqual(vatNumber, dbRow.VatNumber);
    }

    [Test]
    public void UniqNameDbConstraint()
    {
        dynamic existingClient = fixtures["clients"]["one"];

        var exception = Assert.Throws(typeof(Npgsql.PostgresException), () =>
        {
            new PgClients(pgDataSource).Add(existingClient.Name, ValidAddress(), ValidVatNumber());
        });
        StringAssert.Contains("violates unique constraint \"uniq_client_name\"", exception.Message);
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