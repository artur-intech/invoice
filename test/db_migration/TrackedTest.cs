using Intech.Invoice.DbMigration;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class TrackedTest : Base
{
    [Test]
    public void Tracks()
    {
        var migration = new Tracked(new Migration.Fake(), pgDataSource);
        Assert.True(migration.Pending());
        Assert.Zero(AppliedMigrationDbRowCount());

        migration.Apply();

        Assert.False(migration.Pending(), "Migration should not be pending");
        Assert.AreEqual(1, AppliedMigrationDbRowCount(), "A new database row in `applied_migrations` should appear");
        Assert.AreEqual(migration.Id(), pgDataSource.CreateCommand("SELECT id FROM applied_migrations LIMIT 1").ExecuteScalar());
    }

    [Test]
    public void RepresentsAsString()
    {
        var migration = new Migration.Fake();
        var tracked = new Tracked(migration, pgDataSource);

        Assert.AreEqual(migration.ToString(), $"{tracked}");
    }

    long AppliedMigrationDbRowCount()
    {
        return (long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM applied_migrations").ExecuteScalar();
    }
}
