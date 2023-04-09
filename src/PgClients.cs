using System.Collections;
using Npgsql;

namespace Intech.Invoice;

sealed class PgClients : Clients
{
    readonly NpgsqlDataSource pgDataSource;

    public PgClients(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public Client Add(string name, string address, string vatNumber, string email)
    {
        var sql = """
            INSERT INTO clients(
            name,
            address,
            vat_number,
            email)
            VALUES(
            $1,
            $2,
            $3,
            $4)
            RETURNING id;
            """;

        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(name);
        command.Parameters.AddWithValue(address);
        command.Parameters.AddWithValue(vatNumber);
        command.Parameters.AddWithValue(email);
        int id = (int)command.ExecuteScalar();

        return new PgClient(id, pgDataSource);
    }

    public IEnumerator<Client> GetEnumerator()
    {
        using var command = pgDataSource.CreateCommand("SELECT * FROM clients");
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var id = (int)reader["id"];
            var name = (string)reader["name"];
            var address = (string)reader["address"];
            var vatNumber = (string)reader["vat_number"];
            var email = (string)reader["email"];

            yield return new ConstClient(new PgClient(id, pgDataSource), name, address, vatNumber, email);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
