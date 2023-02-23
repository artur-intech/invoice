namespace Intech.Invoice;

interface Client
{
    class Fake : Client
    {
        public int Id()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Fake client";
        }
    }

    int Id();
    string ToString();
}