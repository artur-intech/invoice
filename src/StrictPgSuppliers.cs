namespace Intech.Invoice;

sealed class StrictPgSuppliers : Suppliers
{
    readonly Suppliers origin;

    public StrictPgSuppliers(Suppliers origin)
    {
        this.origin = origin;
    }

    public Supplier Add(string name, string address, string vatNumber, string iban)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Supplier name cannot be empty.");
        }

        if (string.IsNullOrEmpty(address) || string.IsNullOrWhiteSpace(address))
        {
            throw new Exception("Supplier address cannot be empty.");
        }

        if (string.IsNullOrEmpty(iban) || string.IsNullOrWhiteSpace(iban))
        {
            throw new Exception("Supplier IBAN cannot be empty.");
        }

        return origin.Add(name: name, address: address, vatNumber: vatNumber, iban: iban);
    }
}