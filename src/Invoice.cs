namespace Intech.Invoice;

interface Invoice
{
    class Fake : Invoice
    {
        public int id = 1;
        public string clientName = "fake client name";
        public string number = "fake number";
        public DateOnly date = new(1970, 1, 1);
        public DateOnly dueDate = new(1970, 1, 2);
        public long subtotal = 1;
        public long vatAmount = 2;
        public long total = 3;
        public bool paid = true;
        public DateOnly paidDate = new(1970, 1, 3);

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

        public void MarkPaid(DateOnly paidDate)
        {
            throw new NotImplementedException();
        }

        public void WithDetails(Action<int, string, string, DateOnly, DateOnly, long, long, long, bool, DateOnly?> callback)
        {
            callback.Invoke(id, clientName, number, date, dueDate, subtotal, vatAmount, total, paid, paidDate);
        }
    }

    int Id();
    string ToString();
    void MarkPaid(DateOnly paidDate);
    public void WithDetails(Action<int, string, string, DateOnly, DateOnly, long, long, long, bool, DateOnly?> callback);
}
