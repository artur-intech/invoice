using Npgsql;

namespace Intech.Invoice;

sealed class UniqPgSuppliers : Suppliers
{
    readonly Suppliers origin;
    readonly NpgsqlDataSource pgDataSource;

    public UniqPgSuppliers(Suppliers origin, NpgsqlDataSource pgDataSource)
    {
        this.origin = origin;
        this.pgDataSource = pgDataSource;
    }

    public Supplier Add(string name, string address, string vatNumber, string iban)
    {
        if (NameExists(name))
        {
            throw new Exception("Supplier name has already been taken.");
        }

        return origin.Add(name: name, address: address, vatNumber: vatNumber, iban: iban);
    }

    bool NameExists(string name)
    {
        var sql = "SELECT COUNT(*) FROM suppliers WHERE name = $1";
        var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(name);
        return (long)command.ExecuteScalar() > 0;
    }
}