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

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Modify(string newName, string newAddress, string newVatNumber)
        {
            throw new NotImplementedException();
        }

        public string Name()
        {
            throw new NotImplementedException();
        }

        ConsoleMedia Client.Print(ConsoleMedia media)
        {
            throw new NotImplementedException();
        }
    }

    int Id();
    string ToString();
    ConsoleMedia Print(ConsoleMedia media);
    string Name();
    void Delete();
    void Modify(string newName, string newAddress, string newVatNumber);
}