namespace Intech.Invoice;

sealed class ConstInvoice : Invoice
{
    readonly Invoice origin;
    readonly string clientName;
    readonly string number;
    readonly DateOnly date;
    readonly DateOnly dueDate;
    readonly long subtotal;
    readonly long vatAmount;
    readonly long total;
    readonly bool paid;
    readonly DateOnly? paidDate;

    public ConstInvoice(Invoice origin, string clientName, string number, DateOnly date, DateOnly dueDate, long subtotal, long vatAmount, long total, bool paid, DateOnly? paidDate)
    {
        this.origin = origin;
        this.clientName = clientName;
        this.number = number;
        this.date = date;
        this.dueDate = dueDate;
        this.subtotal = subtotal;
        this.vatAmount = vatAmount;
        this.total = total;
        this.paid = paid;
        this.paidDate = paidDate;
    }

    public int Id()
    {
        return origin.Id();
    }

    public override string ToString()
    {
        return origin.ToString();
    }

    public ConsoleMedia Print(ConsoleMedia media)
    {
        return media.With("Id", origin.Id())
                    .With("Client", clientName)
                    .With("Number", number)
                    .With("Date", date)
                    .With("Due date", dueDate)
                    .With("Subtotal", subtotal)
                    .With("VAT amount", vatAmount)
                    .With("Total", total)
                    .With("Paid", paid)
                    .With("Paid on", paidDate);
    }

    public void MarkPaid(DateOnly paidDate)
    {
        origin.MarkPaid(paidDate);
    }
}
