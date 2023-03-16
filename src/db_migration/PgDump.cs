using System.Diagnostics;
using Npgsql;

namespace Intech.Invoice.DbMigration;

class PgDump
{
    readonly string connectionUri;
    readonly NpgsqlDataSource pgDataSource;

    public PgDump(string connectionUri, NpgsqlDataSource pgDataSource)
    {
        this.connectionUri = connectionUri;
        this.pgDataSource = pgDataSource;
    }

    public void DumpToFile(string path)
    {
        var process = new Process();
        process.StartInfo.FileName = "pg_dump";
        process.StartInfo.Arguments = $"-d {connectionUri} -f {path} {ExcludedTablesArguments()}";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();

        var stdout = process.StandardOutput.ReadToEnd();
        Console.WriteLine(stdout);
        process.WaitForExit();
    }

    string ExcludedTablesArguments()
    {
        return string.Join(' ', ExcludedTables().Select(table => $"--exclude-table-data={table}"));
    }

    IEnumerable<string> ExcludedTables()
    {
        using var reader = pgDataSource.CreateCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'").ExecuteReader();
        var tables = new List<string>();

        while (reader.Read())
        {
            tables.Add((string)reader["table_name"]);
        }

        tables.Remove("applied_migrations");
        return tables;
    }
}