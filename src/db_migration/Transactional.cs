namespace Intech.Invoice.DbMigration;

sealed class Transactional : Migration
{
    readonly Migration origin;
    readonly PgTransaction transaction;

    public Transactional(Migration origin, PgTransaction transaction)
    {
        this.origin = origin;
        this.transaction = transaction;
    }

    public void Apply()
    {
        transaction.Wrap(origin.Apply);
    }

    public string Id()
    {
        return origin.Id();
    }

    public bool Pending()
    {
        return origin.Pending();
    }

    public override string ToString()
    {
        return origin.ToString();
    }
}