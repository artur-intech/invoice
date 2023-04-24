namespace Intech.Invoice;

interface Supplier
{
    class Fake : Supplier
    {
        public int id = 1;
        public string name = "fake name";
        public string address = "fake address";
        public string vatNumber = "fake vat number";
        public string iban = "fake iban";
        public string email = "fake email";

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

        public void Modify(string newName, string newAddress, string newVatNumber, string newIban, string newEmail)
        {
            throw new NotImplementedException();
        }

        public void WithDetails(Action<int, string, string, string, string, string> callback)
        {
            callback.Invoke(id, name, address, vatNumber, iban, email);
        }
    }

    int Id();
    string ToString();
    string Name();
    void Delete();
    void Modify(string newName, string newAddress, string newVatNumber, string newIban, string newEmail);
    public void WithDetails(Action<int, string, string, string, string, string> callback);
}
