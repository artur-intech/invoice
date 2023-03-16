using Intech.Invoice.DbMigration;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class MigrationsTest : Base
{
    [Test]
    public void Initializes()
    {
        Directory.Delete(migrationsPath, recursive: true);
        new Migrations(migrationsPath, pgDataSource).Init();
        DirectoryAssert.Exists(migrationsPath);
    }

    [Test]
    public void CreatesEmptyMigration()
    {
        var id = "test";
        var expectedPath = Path.Combine(migrationsPath, $"{id}.pgsql");
        FileAssert.DoesNotExist(expectedPath);

        new Migrations(migrationsPath, pgDataSource).CreateEmpty(new Id.Fake(id));

        FileAssert.Exists(expectedPath);
    }

    [Test]
    public void ReturnsMigrations()
    {
        var thirdMigration = Migration(id: "19700101080003_test");
        var firstMigration = Migration(id: "19700101080001_test");
        var secondMigration = Migration(id: "19700101080002_test");

        var migrations = new Migrations(migrationsPath, pgDataSource);
        var actualMigrations = migrations.ToList();

        CollectionAssert.AreEqual(new List<string> { firstMigration.Id(), secondMigration.Id(), thirdMigration.Id() }, actualMigrations.Select(i => i.Id()).ToList(),
            "Migrations should be sorted by the timestamp");
    }

    [SetUp]
    protected void CreateMigrationsRootDirectory()
    {
        Directory.CreateDirectory(migrationsPath);
    }

    [TearDown]
    protected void DeleteMigrationsRootDirectory()
    {
        new DirectoryInfo(migrationsPath).Delete(recursive: true);
    }
}