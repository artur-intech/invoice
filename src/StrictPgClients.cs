using System.Collections;

namespace Intech.Invoice;

sealed class StrictPgClients : Clients
{
    readonly Clients origin;

    public StrictPgClients(Clients origin)
    {
        this.origin = origin;
    }

    public Client Add(string name, string address, string vatNumber)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Client name cannot be empty.");
        }

        if (string.IsNullOrEmpty(address) || string.IsNullOrWhiteSpace(address))
        {
            throw new Exception("Client address cannot be empty.");
        }

        if (string.IsNullOrEmpty(vatNumber) || string.IsNullOrWhiteSpace(vatNumber))
        {
            throw new Exception("Client VAT number cannot be empty.");
        }

        return origin.Add(name: name, address: address, vatNumber: vatNumber);
    }

    public IEnumerator<Client> GetEnumerator()
    {
        return origin.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)origin).GetEnumerator();
    }
}