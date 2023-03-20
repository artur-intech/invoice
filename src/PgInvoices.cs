using System.Collections;

using Npgsql;

namespace Intech.Invoice;

sealed class PgInvoices : Invoices
{
    readonly NpgsqlDataSource pgDataSource;

    public PgInvoices(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public Invoice Add(string number,
                       DateOnly date,
                       DateOnly dueDate,
                       int vatRate,
                       int supplierId,
                       int clientId)
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

        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(clientId);
        command.Parameters.AddWithValue(number);
        command.Parameters.AddWithValue(date);
        command.Parameters.AddWithValue(dueDate);
        command.Parameters.AddWithValue(vatRate);
        command.Parameters.AddWithValue(supplierId);
        object result = command.ExecuteScalar();

        if (result is null)
        {
            throw new Exception("Database query didn't return invoice id.");
        }

        var id = (int)result;

        return new PgInvoice(id, pgDataSource);
    }

    public IEnumerator<Invoice> GetEnumerator()
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
            GROUP BY
            invoices.id
            ORDER BY
            date DESC
            """;
        using var command = pgDataSource.CreateCommand(sql);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var id = (int)reader["id"];
            var clientName = (string)reader["client_name"];
            var number = (string)reader["number"];
            var date = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date"));
            var dueDate = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("due_date"));
            var subtotal = new Money((long)reader["subtotal"]);
            var vatAmount = new Money((long)reader["vat_amount"]);
            var total = new Money((long)reader["total"]);
            var state = (string)reader["state"];

            yield return new ConstInvoice(new PgInvoice(id, pgDataSource), clientName, number, date, dueDate, subtotal, vatAmount, total, state);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}