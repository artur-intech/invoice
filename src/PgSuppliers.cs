using Npgsql;

namespace Intech.Invoice;

sealed class PgSuppliers : Suppliers
{
    readonly NpgsqlDataSource pgDataSource;

    public PgSuppliers(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public Supplier Add(string name, string address, string vatNumber, string iban)
    {
        var sql = """
            INSERT INTO suppliers(
            name,
            address,
            vat_number,
            iban)
            VALUES(
            $1,
            $2,
            $3,
            $4)
            RETURNING id
            """;

        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(name);
        command.Parameters.AddWithValue(address);
        command.Parameters.AddWithValue(vatNumber);
        command.Parameters.AddWithValue(iban);
        int id = (int)command.ExecuteScalar();

        return new PgSupplier(id, pgDataSource);
    }
}