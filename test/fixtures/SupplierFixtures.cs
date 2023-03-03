using Npgsql;

namespace Intech.Invoice.Test;

class SupplierFixtures
{
    protected NpgsqlDataSource pgDataSource;

    public SupplierFixtures(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public SupplierFixture Create(string name = "test supplier name", string address = "test supplier address", string vatNumber = "test supplier vat number", string iban = "test supplier iban")
    {
        var sql = "INSERT INTO suppliers(name, address, vat_number, iban) VALUES ($1, $2, $3, $4) RETURNING id";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(name);
        command.Parameters.AddWithValue(address);
        command.Parameters.AddWithValue(vatNumber);
        command.Parameters.AddWithValue(iban);
        var createdId = (int)command.ExecuteScalar();

        return Fetch(createdId);
    }

    public SupplierFixture Fetch(int id)
    {
        var sql = "SELECT * FROM suppliers WHERE id = $1";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();
        reader.Read();
        var name = (string)reader["name"];
        var address = (string)reader["address"];
        var vatNumber = (string)reader["vat_number"];
        var iban = (string)reader["iban"];

        return new SupplierFixture() { Id = id, Name = name, Address = address, VatNumber = vatNumber, Iban = iban };
    }
}