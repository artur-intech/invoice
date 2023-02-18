using Npgsql;

namespace Intech.Invoice.Test;

class LineItemFixtures
{
    protected NpgsqlDataSource pgDataSource;

    public LineItemFixtures(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public LineItemFixture Create(int invoiceId, string name = "test line item name", int price = 100, int quantity = 150)
    {
        var sql = "INSERT INTO line_items(invoice_id, name, price, quantity) VALUES ($1, $2, $3, $4) RETURNING id";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(invoiceId);
        command.Parameters.AddWithValue(name);
        command.Parameters.AddWithValue(price);
        command.Parameters.AddWithValue(quantity);
        var createdId = (int)command.ExecuteScalar();

        return Fetch(createdId);
    }

    LineItemFixture Fetch(int id)
    {
        var sql = "SELECT * FROM line_items WHERE id = $1";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();
        reader.Read();
        var name = (string)reader["name"];
        var price = (short)reader["price"];
        var quantity = (short)reader["quantity"];

        return new LineItemFixture() { Id = id, Name = name, Price = price, Quantity = quantity };
    }
}