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

    public Supplier Add(string name, string address, string vatNumber, string iban, string email)
    {
        if (NameTaken(name))
        {
            throw new Exception("Supplier name has already been taken.");
        }

        return origin.Add(name, address, vatNumber, iban, email);
    }

    bool NameTaken(string name)
    {
        var cmd = pgDataSource.CreateCommand("SELECT id FROM suppliers WHERE name = $1");
        cmd.Parameters.AddWithValue(name);
        return cmd.ExecuteScalar() is not null;
    }
}
