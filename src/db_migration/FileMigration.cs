using Npgsql;

namespace Intech.Invoice.DbMigration;

sealed class FileMigration : Migration
{
    readonly string path;
    readonly NpgsqlDataSource pgDataSource;

    public FileMigration(string path, NpgsqlDataSource pgDataSource)
    {
        this.path = path;
        this.pgDataSource = pgDataSource;
    }

    public void Apply()
    {
        pgDataSource.CreateCommand(Sql()).ExecuteNonQuery();
    }

    public bool Pending()
    {
        return true;
    }

    public override string ToString()
    {
        return $"{Id()}";
    }

    public string Id()
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    string Sql()
    {
        return File.ReadAllText(path);
    }
}
