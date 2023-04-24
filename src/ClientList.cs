namespace Intech.Invoice;

sealed class ClientList : DelimitedList
{
    public ClientList(IEnumerable<Client> list) : base(list) { }

    protected override void PrintBody()
    {
        foreach (var client in list.Cast<Client>())
        {
            PrintDelimiter();

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
}
