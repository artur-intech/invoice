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

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public string Name()
        {
            throw new NotImplementedException();
        }

        ConsoleMedia Supplier.Print(ConsoleMedia media)
        {
            throw new NotImplementedException();
        }

        public void Modify(string newName, string newAddress, string newVatNumber, string newIban, string newEmail)
        {
            throw new NotImplementedException();
        }
    }

    int Id();
    string ToString();
    ConsoleMedia Print(ConsoleMedia media);
    string Name();
    void Delete();
    void Modify(string newName, string newAddress, string newVatNumber, string newIban, string newEmail);
}
