namespace Intech.Invoice;

interface Clients
{
    Client Add(string name, string address, string vatNumber);
}