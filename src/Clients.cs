namespace Intech.Invoice;

interface Clients
{
    class Fake : Clients
    {
        public Client Add(string name, string address, string vatNumber)
        {
            return new Client.Fake();
        }
    }

    Client Add(string name, string address, string vatNumber);
}