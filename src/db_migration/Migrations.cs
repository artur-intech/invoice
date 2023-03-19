using System.Collections;
using Npgsql;

namespace Intech.Invoice.DbMigration;

sealed class Migrations : IEnumerable<Migration>
{
    readonly string path;
    readonly Func<string, Migration> pathToMigration;

    public Migrations(string path, NpgsqlDataSource pgDataSource)
    {
        this.path = path;
        // Consider returning `pgDataSource` from FileTransaction
        pathToMigration = (path) => new Transactional(new Tracked(new FileMigration(path, pgDataSource), pgDataSource), new PgTransaction(pgDataSource));
    }

    public void Init()
    {
        Directory.CreateDirectory(path);
        using (File.Create(Path.Combine(path, ".gitkeep"))) { };
    }

    public void CreateEmpty(Id id)
    {
        var filename = $"{id}.pgsql";
        var filepath = Path.Combine(path, filename);
        using (File.Create(filepath))
        {
        }
    }

    public IEnumerator<Migration> GetEnumerator()
    {
        var directory = new DirectoryInfo(path);
        var files = directory.GetFiles($"*.pgsql").OrderBy(f => f.Name);

        foreach (var file in files)
        {
            yield return pathToMigration(file.FullName);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}