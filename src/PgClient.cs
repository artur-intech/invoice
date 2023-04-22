using Npgsql;

namespace Intech.Invoice;

sealed class PgClient : Client
{
    readonly int id;
    readonly NpgsqlDataSource pgDataSource;

    public PgClient(int id, NpgsqlDataSource pgDataSource)
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

    public void Modify(string newName, string newAddress, string newVatNumber)
    {
        var sql = "UPDATE clients SET name = $1, address = $2, vat_number = $3 WHERE id = $4";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(newName);
        command.Parameters.AddWithValue(newAddress);
        command.Parameters.AddWithValue(newVatNumber);
        command.Parameters.AddWithValue(id);
        command.ExecuteNonQuery();
    }

    public string Name()
    {
        using var command = pgDataSource.CreateCommand("SELECT name FROM clients WHERE id = $1");
        command.Parameters.AddWithValue(id);
        return (string)command.ExecuteScalar();
    }

    public void Delete()
    {
        if (Invoiced())
        {
            throw new Exception("Client has invoices and therefore cannot be deleted.");
        }

        using var command = pgDataSource.CreateCommand("DELETE FROM clients WHERE id = $1");
        command.Parameters.AddWithValue(id);
        command.ExecuteNonQuery();
    }

    public void WithDetails(Action<int, string, string, string, string> callback)
    {
        using var cmd = pgDataSource.CreateCommand("SELECT * FROM clients WHERE id = $1");
        cmd.Parameters.AddWithValue(id);
        using var reader = cmd.ExecuteReader();
        reader.Read();

        callback.Invoke((int)reader["id"], (string)reader["name"], (string)reader["address"],
            (string)reader["vat_number"], (string)reader["email"]);
    }

    bool Invoiced()
    {
        var sql = "SELECT COUNT(*) > 0 FROM invoices WHERE client_id = $1";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(id);
        return (bool)command.ExecuteScalar();
    }
}
