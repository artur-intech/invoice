using System.Collections;
using Npgsql;

namespace Intech.Invoice;

// Having separate class enables to test database constraints
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

    public IEnumerator<Supplier> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    bool NameTaken(string name)
    {
        var cmd = pgDataSource.CreateCommand("SELECT id FROM suppliers WHERE name = $1");
        cmd.Parameters.AddWithValue(name);
        return cmd.ExecuteScalar() is not null;
    }
}
