using System.Collections;
using Npgsql;

namespace Intech.Invoice;

sealed class PgSuppliers : Suppliers, IEnumerable<Supplier>
{
    readonly NpgsqlDataSource pgDataSource;

    public PgSuppliers(NpgsqlDataSource pgDataSource)
    {
        this.pgDataSource = pgDataSource;
    }

    public Supplier Add(string name, string address, string vatNumber, string iban, string email)
    {
        var sql = """
            INSERT INTO suppliers(
            name,
            address,
            vat_number,
            iban,
            email)
            VALUES(
            $1,
            $2,
            $3,
            $4,
            $5)
            RETURNING id
            """;

        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(name);
        command.Parameters.AddWithValue(address);
        command.Parameters.AddWithValue(vatNumber);
        command.Parameters.AddWithValue(iban);
        command.Parameters.AddWithValue(email);
        int id = (int)command.ExecuteScalar();

        return new PgSupplier(id, pgDataSource);
    }

    public IEnumerator<Supplier> GetEnumerator()
    {
        using var command = pgDataSource.CreateCommand("SELECT * FROM suppliers");
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var rawData = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);
            yield return new ConstSupplier(rawData);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
