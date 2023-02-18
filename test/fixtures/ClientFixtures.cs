using Npgsql;

namespace Intech.Invoice.Test;

class ClientFixtures
{
    protected NpgsqlDataSource pgDataSource;

    public ClientFixtures(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public ClientFixture Create(string name = "test client name", string address = "test client address", string vatNumber = "test client vat number")
    {
        var sql = "INSERT INTO clients(name, address, vat_number) VALUES ($1, $2, $3) RETURNING id";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(name);
        command.Parameters.AddWithValue(address);
        command.Parameters.AddWithValue(vatNumber);
        var createdId = (int)command.ExecuteScalar();

        return Fetch(createdId);
    }

    ClientFixture Fetch(int id)
    {
        var sql = "SELECT * FROM clients WHERE id = $1";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();
        reader.Read();
        var name = (string)reader["name"];
        var address = (string)reader["address"];
        var vatNumber = (string)reader["vat_number"];

        return new ClientFixture() { Id = id, Name = name, Address = address, VatNumber = vatNumber };
    }
}