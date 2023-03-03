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

        ConsoleMedia Client.Print(ConsoleMedia media)
        {
            throw new NotImplementedException();
        }
    }

    int Id();
    string ToString();
    ConsoleMedia Print(ConsoleMedia media);
}