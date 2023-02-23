namespace Intech.Invoice;

interface Supplier
{
    class Fake : Supplier
    {
        public int Id()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Fake supplier";
        }
    }

    int Id();
    string ToString();
}