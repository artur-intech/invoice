using System.Collections;
using Npgsql;

namespace Intech.Invoice;

// Having separate class enables to test database constraints
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
        if (NameTaken(name))
        {
            throw new Exception("Client name has already been taken.");
        }

        return origin.Add(name: name, address: address, vatNumber: vatNumber);
    }

    bool NameTaken(string name)
    {
        var cmd = pgDataSource.CreateCommand("SELECT id FROM clients WHERE name = $1");
        cmd.Parameters.AddWithValue(name);
        return cmd.ExecuteScalar() is not null;
    }

    public IEnumerator<Client> GetEnumerator()
    {
        return origin.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)origin).GetEnumerator();
    }
}