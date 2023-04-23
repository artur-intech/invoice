namespace Intech.Invoice;

sealed class ConsoleDelimitedSupplierList<T>
{
    readonly IEnumerable<T> list;

    public ConsoleDelimitedSupplierList(IEnumerable<T> list)
    {
        this.list = list;
    }

    public void Print()
    {
        Console.WriteLine($"Records total: {list.ToList().Count}");

        foreach (dynamic listItem in list)
        {
            Console.WriteLine(Delimiter());

            listItem.WithDetails((Action<int, string, string, string, string, string>)((int id, string name, string address, string vatNumber, string iban, string email) =>
            {
                Console.WriteLine($"Id: {id}");
                Console.WriteLine($"Name: {name}");
                Console.WriteLine($"Address: {address}");
                Console.WriteLine($"VAT number: {vatNumber}");
                Console.WriteLine($"IBAN: {iban}");
                Console.WriteLine($"Email: {email}");
            }));
        }
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
