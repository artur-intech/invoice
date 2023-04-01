using Npgsql;

namespace Intech.Invoice.DbMigration;

sealed class Tracked : Migration
{
    readonly Migration origin;
    readonly NpgsqlDataSource pgDataSource;

    public Tracked(Migration origin, NpgsqlDataSource pgDataSource)
    {
        this.origin = origin;
        this.pgDataSource = pgDataSource;
    }

    public void Apply()
    {
        origin.Apply();
        Track();
    }

    public string Id()
    {
        return origin.Id();
    }

    public bool Pending()
    {
        // Consider caching since it is mostly called from collections.
        using var command = pgDataSource.CreateCommand("SELECT COUNT(*) FROM applied_migrations WHERE id = $1");
        command.Parameters.AddWithValue(Id());

        return (long)command.ExecuteScalar() == 0;
    }

    public override string ToString()
    {
        return origin.ToString();
    }

    void Track()
    {
        using var command = pgDataSource.CreateCommand("INSERT INTO applied_migrations VALUES($1)");
        command.Parameters.AddWithValue(Id());
        command.ExecuteNonQuery();
    }
}
