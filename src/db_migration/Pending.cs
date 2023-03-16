using System.Collections.Immutable;

namespace Intech.Invoice.DbMigration;

sealed class Pending
{
    readonly IEnumerable<Migration> origin;

    public Pending(IEnumerable<Migration> origin)
    {
        this.origin = origin;
    }

    public void Apply(Action<IEnumerable<Migration>> anyCallback, Action nothingCallback)
    {
        if (Filtered().Any())
        {
            var applied = ImmutableList<Migration>.Empty;

            Filtered().AsParallel().ForAll(migration =>
            {
                migration.Apply();
                applied = applied.Add(migration);
            });

            anyCallback(applied);
        }
        else nothingCallback();
    }

    IEnumerable<Migration> Filtered()
    {
        return origin.Where(migration => migration.Pending());
    }
}