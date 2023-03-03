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

    public void Modify(string newName, string newAddress, string newVatNumber, string newIban)
    {
        var sql = "UPDATE suppliers SET name = $1, address = $2, vat_number = $3, iban = $4 WHERE id = $5";
        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(newName);
        command.Parameters.AddWithValue(newAddress);
        command.Parameters.AddWithValue(newVatNumber);
        command.Parameters.AddWithValue(newIban);
        command.Parameters.AddWithValue(id);
        command.ExecuteNonQuery();
    }

    string Name()
    {
        using var command = pgDataSource.CreateCommand("SELECT name FROM suppliers WHERE id = $1");
        command.Parameters.AddWithValue(id);
        return (string)command.ExecuteScalar();
    }
}