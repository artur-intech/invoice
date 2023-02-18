using Npgsql;

namespace Intech.Invoice;

sealed class PgClient : Client
{
    readonly int id;
    readonly NpgsqlDataSource pgDataSource;

    public PgClient(int id, NpgsqlDataSource pgDataSource)
    {
        this.id = id;
        this.pgDataSource = pgDataSource;
    }

    public int Id()
    {
        return id;
    }

    public override string ToString()
    {
        return Name();
    }

    string Name()
    {
        using var command = pgDataSource.CreateCommand("SELECT name FROM clients WHERE id = $1");
        command.Parameters.AddWithValue(id);
        return (string)command.ExecuteScalar();
    }
}