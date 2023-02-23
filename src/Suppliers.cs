namespace Intech.Invoice;

interface Suppliers
{
    class Fake : Suppliers
    {
        public Supplier Add(string name, string address, string vatNumber, string iban)
        {
            return new Supplier.Fake();
        }
    }

    Supplier Add(string name, string address, string vatNumber, string iban);
}