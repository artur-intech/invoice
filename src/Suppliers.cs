using System.Collections;

namespace Intech.Invoice;

interface Suppliers : IEnumerable<Supplier>
{
    class Fake : Suppliers
    {
        public Supplier Add(string name, string address, string vatNumber, string iban)
        {
            return new Supplier.Fake();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<Supplier> IEnumerable<Supplier>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    Supplier Add(string name, string address, string vatNumber, string iban);
}