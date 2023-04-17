using System.Collections;
using Npgsql;

namespace Intech.Invoice;

sealed class PgSuppliers : Suppliers
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
            var id = (int)reader["id"];
            var name = (string)reader["name"];
            var address = (string)reader["address"];
            var vatNumber = (string)reader["vat_number"];
            var iban = (string)reader["iban"];
            var email = (string)reader["email"];

            yield return new ConstSupplier(new PgSupplier(id, pgDataSource), name, address, vatNumber, iban, email);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
