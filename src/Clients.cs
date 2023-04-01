using System.Collections;

namespace Intech.Invoice;

interface Clients : IEnumerable<Client>
{
    class Fake : Clients
    {
        public Client Add(string name, string address, string vatNumber)
        {
            return new Client.Fake();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<Client> IEnumerable<Client>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    Client Add(string name, string address, string vatNumber);
}
