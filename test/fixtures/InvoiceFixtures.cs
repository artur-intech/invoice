using Npgsql;

namespace Intech.Invoice.Test;

class InvoiceFixtures
{
    protected NpgsqlDataSource pgDataSource;

    public InvoiceFixtures(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public int Create(int supplierId, int clientId, DateOnly date = default, DateOnly? dueDate = null, string? number = null, int vatRate = 20)
    {
        var sql = """
            INSERT INTO invoices(
            client_id,
            number,
            date,
            due_date,
            vat_rate,
            supplier_name,
            supplier_address,
            supplier_vat_number,
            supplier_iban,
            client_name,
            client_address,
            client_vat_number)
            SELECT
            $1,
            $2,
            $3,
            $4,
            $5,
            s.name,
            s.address,
            s.vat_number,
            s.iban,
            c.name,
            c.address,
            c.vat_number
            FROM
            suppliers s,
            clients c
            WHERE
            s.id = $6 AND c.id = $1
            RETURNING id
            """;

        if (number is null)
        {
            var rand = new Random();
            number = rand.Next().ToString();
        }

        dueDate ??= date;

        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(clientId);
        command.Parameters.AddWithValue(number);
        command.Parameters.AddWithValue(date);
        command.Parameters.AddWithValue(dueDate ?? new DateOnly(1970, 01, 02));
        command.Parameters.AddWithValue(vatRate);
        command.Parameters.AddWithValue(supplierId);
        var createdId = (int)command.ExecuteScalar();

        return createdId;
    }

    public InvoiceFixture Fetch(int id)
    {
        var sql = """
            SELECT
            invoices.*,
            SUM(price * quantity::int) AS subtotal,
            (SUM(price * quantity::int) * vat_rate) / 100 AS vat_amount,
            SUM(price * quantity::int) + ((SUM(price * quantity::int) * vat_rate) / 100) AS total
            FROM
            invoices
            LEFT JOIN
            line_items ON invoices.id = line_items.invoice_id
            WHERE
            invoices.id = $1
            GROUP BY
            invoices.id
            """;
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();
        reader.Read();
        var number = (string)reader["number"];
        var date = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date"));
        var dueDate = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("due_date"));
        var vatRate = (short)reader["vat_rate"];
        var subtotal = (long)reader["subtotal"];
        var vatAmount = (long)reader["vat_amount"];
        var total = (long)reader["total"];
        var supplierName = (string)reader["supplier_name"];
        var state = (string)reader["state"];

        return new InvoiceFixture() { Id = id, Number = number, Date = date, DueDate = dueDate, VatRate = vatRate, Subtotal = subtotal, VatAmount = vatAmount, Total = total, SupplierName = supplierName, State = state };
    }
}