namespace Intech.Invoice;

interface Suppliers
{
    Supplier Add(string name, string address, string vatNumber, string iban);
}