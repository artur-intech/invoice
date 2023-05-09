namespace Intech.Invoice;

interface Clients
{
    class Fake : Clients
    {
        public Client Add(string name, string address, string vatNumber, string email)
        {
            return new Client.Fake();
        }
    }

    Client Add(string name, string address, string vatNumber, string email);
}
