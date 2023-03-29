using System.Collections;
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

        return origin.Add(name, address, vatNumber, iban);
    }

    public IEnumerator<Supplier> GetEnumerator()
    {
        return origin.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)origin).GetEnumerator();
    }

    bool NameExists(string name)
    {
        var cmd = pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers WHERE name = $1");
        cmd.Parameters.AddWithValue(name);
        return (long)cmd.ExecuteScalar() > 0;
    }
}