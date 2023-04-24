namespace Intech.Invoice;

sealed class SupplierList : DelimitedList
{
    public SupplierList(IEnumerable<Supplier> list) : base(list) { }

    protected override void PrintBody()
    {
        foreach (var supplier in list.Cast<Supplier>())
        {
            PrintDelimiter();

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
}
