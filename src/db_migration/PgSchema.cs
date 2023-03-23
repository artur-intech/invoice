using System.Collections.Immutable;
using Npgsql;

namespace Intech.Invoice.DbMigration;

class PgSchema : Schema
{
    readonly string path;
    readonly NpgsqlDataSource pgDataSource;
    readonly DumpUtil dumpUtil;

    public PgSchema(string path, NpgsqlDataSource pgDataSource, DumpUtil dumpUtil)
    {
        this.path = path;
        this.pgDataSource = pgDataSource;
        this.dumpUtil = dumpUtil;
    }

    public void Generate()
    {
        dumpUtil.DumpToFile(path, ExcludedDataTables());
    }
    public void Regenerate() => Generate();

    IEnumerable<string> ExcludedDataTables()
    {
        using var cmd = pgDataSource.CreateCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_name != 'applied_migrations'");
        using var reader = cmd.ExecuteReader();
        var tables = ImmutableList<string>.Empty;

        while (reader.Read())
        {
            tables = tables.Add((string)reader["table_name"]);
        }

        return tables;
    }
}