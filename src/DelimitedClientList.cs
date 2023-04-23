namespace Intech.Invoice;

sealed class DelimitedClientList
{
    readonly IEnumerable<Client> invoices;

    public DelimitedClientList(IEnumerable<Client> invoices)
    {
        this.invoices = invoices;
    }

    public void Print()
    {
        Console.WriteLine($"Records total: {invoices.ToList().Count}");

        foreach (var client in invoices)
        {
            Console.WriteLine(Delimiter());

            client.WithDetails((int id, string name, string address, string vatNumber, string email) =>
            {
                Console.WriteLine($"Id: {id}");
                Console.WriteLine($"Name: {name}");
                Console.WriteLine($"Address: {address}");
                Console.WriteLine($"VAT number: {vatNumber}");
                Console.WriteLine($"Email: {email}");
            });
        }
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
