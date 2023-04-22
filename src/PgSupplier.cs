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

    public void Delete()
    {
        if (Invoiced()) throw new Exception("Supplier has invoices and therefore cannot be deleted.");

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

    public void WithDetails(Action<int, string, string, string, string, string> callback)
    {
        using var cmd = pgDataSource.CreateCommand("SELECT * FROM suppliers WHERE id = $1");
        cmd.Parameters.AddWithValue(id);
        using var reader = cmd.ExecuteReader();
        reader.Read();

        callback.Invoke((int)reader["id"], (string)reader["name"], (string)reader["address"],
            (string)reader["vat_number"], (string)reader["iban"], (string)reader["email"]);
    }

    bool Invoiced()
    {
        using var cmd = pgDataSource.CreateCommand("SELECT COUNT(*) > 0 FROM invoices WHERE supplier_id = $1");
        cmd.Parameters.AddWithValue(id);
        return (bool)cmd.ExecuteScalar();
    }
}
