namespace Intech.Invoice;

sealed class DelimitedSupplierList
{
    readonly IEnumerable<Supplier> suppliers;

    public DelimitedSupplierList(IEnumerable<Supplier> suppliers)
    {
        this.suppliers = suppliers;
    }

    public void Print()
    {
        Console.WriteLine($"Records total: {suppliers.ToList().Count}");

        foreach (var supplier in suppliers)
        {
            Console.WriteLine(Delimiter());

            supplier.WithDetails((int id, string name, string address, string vatNumber, string iban, string email) =>
            {
                Console.WriteLine($"Id: {id}");
                Console.WriteLine($"Name: {name}");
                Console.WriteLine($"Address: {address}");
                Console.WriteLine($"VAT number: {vatNumber}");
                Console.WriteLine($"IBAN: {iban}");
                Console.WriteLine($"Email: {email}");
            });
        }
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
