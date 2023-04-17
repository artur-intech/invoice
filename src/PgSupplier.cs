using Npgsql;

namespace Intech.Invoice;

sealed class PgSupplier : Supplier
{
    readonly int id;
    readonly NpgsqlDataSource pgDataSource;

    public PgSupplier(int id, NpgsqlDataSource pgDataSource)
    {
        this.id = id;
        this.pgDataSource = pgDataSource;
    }

    public int Id()
    {
        return id;
    }

    public override string ToString()
    {
        return Name();
    }

    public void Modify(string newName, string newAddress, string newVatNumber, string newIban, string newEmail)
    {
        var sql = "UPDATE suppliers SET name = $1, address = $2, vat_number = $3, iban = $4, email = $5 WHERE id = $6";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(newName);
        command.Parameters.AddWithValue(newAddress);
        command.Parameters.AddWithValue(newVatNumber);
        command.Parameters.AddWithValue(newIban);
        command.Parameters.AddWithValue(newEmail);
        command.Parameters.AddWithValue(id);
        command.ExecuteNonQuery();
    }

    public ConsoleMedia Print(ConsoleMedia media)
    {
        var sql = "SELECT * FROM suppliers WHERE id = $1";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();

        reader.Read();
        var name = reader["name"];
        var address = reader["address"];
        var vatNumber = reader["vat_number"];
        var iban = reader["iban"];
        var email = reader["email"];

        return media.With("Id", id)
                    .With("Name", name)
                    .With("Address", address)
                    .With("VAT number", vatNumber)
                    .With("IBAN", iban)
                    .With("Email", email);
    }

    public void Delete()
    {
        using var command = pgDataSource.CreateCommand("DELETE FROM suppliers WHERE id = $1");
        command.Parameters.AddWithValue(id);
        command.ExecuteNonQuery();
    }

    public string Name()
    {
        using var command = pgDataSource.CreateCommand("SELECT name FROM suppliers WHERE id = $1");
        command.Parameters.AddWithValue(id);
        return (string)command.ExecuteScalar();
    }
}
