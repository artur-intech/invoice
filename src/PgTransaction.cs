using Npgsql;

namespace Intech.Invoice;

sealed class PgTransaction
{
    readonly NpgsqlDataSource pgDataSource;

    public PgTransaction(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public void Wrap(Action callback)
    {
        using var connection = pgDataSource.OpenConnection();
        using var transaction = connection.BeginTransaction();

        callback();

        transaction.Commit();
    }
}
