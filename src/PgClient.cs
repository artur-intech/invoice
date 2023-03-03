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

    public ConsoleMedia Print(ConsoleMedia media)
    {
        using var command = pgDataSource.CreateCommand("SELECT * FROM clients WHERE id = $1");
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();

        reader.Read();
        var name = reader["name"];
        var address = reader["address"];
        var vatNumber = reader["vat_number"];

        return media.With("Id", id)
                    .With("Name", name)
                    .With("Address", address)
                    .With("VAT number", vatNumber);
    }

    string Name()
    {
        using var command = pgDataSource.CreateCommand("SELECT name FROM clients WHERE id = $1");
        command.Parameters.AddWithValue(id);
        return (string)command.ExecuteScalar();
    }
}