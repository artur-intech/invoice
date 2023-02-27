using System.Collections.Immutable;
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

    public IEnumerable<PgInvoice> Fetch()
    {
        using var command = pgDataSource.CreateCommand("SELECT id FROM invoices");
        using var reader = command.ExecuteReader();
        var pgInvoices = ImmutableList<PgInvoice>.Empty;

        while (reader.Read())
        {
            var invoiceId = (int)reader["id"];
            var pgInvoice = new PgInvoice(invoiceId, pgDataSource);
            pgInvoices = pgInvoices.Add(pgInvoice);
        }

        return pgInvoices;
    }
}