using Npgsql;

namespace Intech.Invoice;

sealed class PgLineItems : LineItems
{
    readonly NpgsqlDataSource pgDataSource;

    public PgLineItems(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public int Add(int invoiceId, string name, int price, int quantity)
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
            RETURNING id
            """;

        using var cmd = pgDataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue(invoiceId);
        cmd.Parameters.AddWithValue(name);
        cmd.Parameters.AddWithValue(price);
        cmd.Parameters.AddWithValue(quantity);
        object result = cmd.ExecuteScalar();

        if (result is null)
        {
            throw new Exception("Database query didn't return line item id.");
        }

        return (int)result;
    }
}
