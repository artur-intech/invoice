using NUnit.Framework;

namespace Intech.Invoice.Test;

class FileMigrationTest : Base
{
    [Test]
    public void Applies()
    {
        var migration = Migration(sql: "CREATE TABLE migration_sql()");
        migration.Apply();
        AssertMigrationSqlExecuted();
    }

    [Test]
    public void RepresentsAsString()
    {
        var migration = Migration();
        Assert.AreEqual(migration.Id(), $"{migration}");
    }

    [SetUp]
    protected void CreateMigrationsRootDirectory()
    {
        Directory.CreateDirectory(migrationsPath);
    }

    [TearDown]
    protected void TearDown()
    {
        new DirectoryInfo(migrationsPath).Delete(recursive: true);
        pgDataSource.CreateCommand("DROP TABLE IF EXISTS migration_sql").ExecuteNonQuery();
    }

    void AssertMigrationSqlExecuted()
    {
        const string sql = "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'migration_sql')";
        Assert.True((bool)pgDataSource.CreateCommand(sql).ExecuteScalar(), "Migration should be applied");
    }
}