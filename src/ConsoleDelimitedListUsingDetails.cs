namespace Intech.Invoice;

sealed class ConsoleDelimitedListUsingDetails<T>
{
    readonly IEnumerable<T> list;

    public ConsoleDelimitedListUsingDetails(IEnumerable<T> list)
    {
        this.list = list;
    }

    public void Print()
    {
        Console.WriteLine($"Records total: {list.ToList().Count}");

        foreach (dynamic listItem in list)
        {
            Console.WriteLine(Delimiter());

            listItem.WithDetails((Action<int, string, string, string, string>)((int id, string name, string address, string vatNumber, string email) =>
            {
                Console.WriteLine($"Id: {id}");
                Console.WriteLine($"Name: {name}");
                Console.WriteLine($"Address: {address}");
                Console.WriteLine($"VAT number: {vatNumber}");
                Console.WriteLine($"Email: {email}");
            }));
        }
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
