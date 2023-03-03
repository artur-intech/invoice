namespace Intech.Invoice;

sealed class ConsoleClientList
{
    readonly IEnumerable<Client> clients;

    public ConsoleClientList(IEnumerable<Client> clients)
    {
        this.clients = clients;
    }

    public void Print()
    {
        var i = 0;
        var delimiter = new string('-', 50);

        foreach (var client in clients)
        {
            if (i != 0)
            {
                Console.Write(Environment.NewLine + delimiter + Environment.NewLine);
            }

            Console.Write(client.Print(new ConsoleMedia()).Text());
            i++;
        }
    }
}