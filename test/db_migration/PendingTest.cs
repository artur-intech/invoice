using Intech.Invoice.DbMigration;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class PendingTest
{
    [Test]
    public void AppliesPendingMigrations()
    {
        var pendingMigration = new Migration.Fake(pending: true);
        Assert.True(pendingMigration.Pending());
        var pendingMigrations = new Pending(new List<Migration>() { pendingMigration });

        pendingMigrations.Apply(whenAny: (applied) =>
        {
            Assert.AreEqual(1, applied.Count());

            foreach (var migration in applied)
            {
                Assert.False(migration.Pending());
            }
        }, whenNone: () => { });
    }

    [Test]
    public void ExecutesCallbackWhenNoPendingMigrationsFound()
    {
        var applied = new Migration.Fake(pending: false);
        var pending = new Pending(new List<Migration>() { applied });
        var callbackExecuted = false;

        pending.Apply(whenAny: (_) => { }, whenNone: () => { callbackExecuted = true; });

        Assert.True(callbackExecuted);
    }
}
