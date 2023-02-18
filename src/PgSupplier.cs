using Npgsql;

namespace Intech.Invoice;

sealed class PgSupplier : Supplier
{
    readonly int id;
    readonly NpgsqlDataSource pgDataSource;

    public PgSupplier(int id, NpgsqlDataSource pgDataSource)
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
        using var command = pgDataSource.CreateCommand("SELECT name FROM suppliers WHERE id = $1");
        command.Parameters.AddWithValue(id);
        return (string)command.ExecuteScalar();
    }
}