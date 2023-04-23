namespace Intech.Invoice;

interface Invoice
{
    int Id();
    string ToString();
    void MarkPaid(DateOnly paidDate);
    public void WithDetails(Action<int, string, string, DateOnly, DateOnly, long, long, long, bool, DateOnly?> callback);
}
