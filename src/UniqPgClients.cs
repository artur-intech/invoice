using Npgsql;

namespace Intech.Invoice;

sealed class UniqPgClients : Clients
{
    readonly Clients origin;
    readonly NpgsqlDataSource pgDataSource;

    public UniqPgClients(Clients origin, NpgsqlDataSource pgDataSource)
    {
        this.origin = origin;
        this.pgDataSource = pgDataSource;
    }

    public Client Add(string name, string address, string vatNumber)
    {
        if (NameExists(name))
        {
            throw new Exception("Client name has already been taken.");
        }

        return origin.Add(name: name, address: address, vatNumber: vatNumber);
    }

    bool NameExists(string name)
    {
        var sql = "SELECT COUNT(*) FROM clients WHERE name = $1";
        var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(name);
        return (long)command.ExecuteScalar() > 0;
    }
}