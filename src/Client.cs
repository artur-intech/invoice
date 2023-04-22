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

        public void WithDetails(Action<int, string, string, string, string> callback)
        {
            throw new NotImplementedException();
        }
    }

    int Id();
    string ToString();
    string Name();
    void Delete();
    void Modify(string newName, string newAddress, string newVatNumber);
    public void WithDetails(Action<int, string, string, string, string> callback);
}
