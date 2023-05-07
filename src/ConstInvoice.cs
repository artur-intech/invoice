namespace Intech.Invoice;

sealed class ConstInvoice : Invoice
{
    readonly IDictionary<string, object> rawDbData;

    public ConstInvoice(IDictionary<string, object> rawDbData)
    {
        this.rawDbData = rawDbData;
    }

    public int Id()
    {
        return (int)rawDbData["id"];
    }

    public override string ToString()
    {
        return Number();
    }

    public void MarkPaid(DateOnly paidDate)
    {
        throw new NotImplementedException();
    }

    public void WithDetails(Action<int, string, string, DateOnly, DateOnly, long, long, long, bool, DateOnly?> callback)
    {
        callback.Invoke(Id(), ClientName(), Number(), Date(), DueDate(), Subtotal(), VatAmount(), Total(), Paid(),
            PaidDate());
    }

    string Number()
    {
        return (string)rawDbData["number"];
    }

    string ClientName()
    {
        return (string)rawDbData["client_name"];
    }

    DateOnly Date()
    {
        return DateOnly.FromDateTime((DateTime)rawDbData["date"]);
    }

    DateOnly DueDate()
    {
        return DateOnly.FromDateTime((DateTime)rawDbData["due_date"]);
    }

    long Subtotal()
    {
        return (long)rawDbData["subtotal"];
    }

    long VatAmount()
    {
        return (long)rawDbData["vat_amount"];
    }

    long Total()
    {
        return (long)rawDbData["total"];
    }

    bool Paid()
    {
        return (bool)rawDbData["paid"];
    }

    DateOnly? PaidDate()
    {
        return rawDbData["paid_date"] is not DBNull ? DateOnly.FromDateTime((DateTime)rawDbData["paid_date"]) : null;
    }
}
