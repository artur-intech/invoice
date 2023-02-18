using Npgsql;
namespace Intech.Invoice;

sealed class PgClients : Clients
{
    readonly NpgsqlDataSource pgDataSource;

    public PgClients(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public Client Add(string name, string address, string vatNumber)
    {
        var sql = """
            INSERT INTO clients(
            name,
            address,
            vat_number)
            VALUES(
            $1,
            $2,
            $3)
            RETURNING id;
            """;

        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(name);
        command.Parameters.AddWithValue(address);
        command.Parameters.AddWithValue(vatNumber);
        int id = (int)command.ExecuteScalar();

        return new PgClient(id, pgDataSource);
    }
}