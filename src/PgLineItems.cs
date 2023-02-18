using Npgsql;

namespace Intech.Invoice;

sealed class PgLineItems : LineItems
{
    readonly NpgsqlDataSource pgDataSource;

    public PgLineItems(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public void Add(int invoiceId, string name, int price, int quantity)
    {
        var sql = """
            INSERT INTO line_items(
            invoice_id,
            name,
            price,
            quantity)
            VALUES(
            $1,
            $2,
            $3,
            $4)
            """;

        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(invoiceId);
        command.Parameters.AddWithValue(name);
        command.Parameters.AddWithValue(price);
        command.Parameters.AddWithValue(quantity);
        command.ExecuteScalar();
    }
}