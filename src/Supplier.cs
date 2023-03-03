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

        ConsoleMedia Supplier.Print(ConsoleMedia media)
        {
            throw new NotImplementedException();
        }
    }

    int Id();
    string ToString();
    ConsoleMedia Print(ConsoleMedia media);
}