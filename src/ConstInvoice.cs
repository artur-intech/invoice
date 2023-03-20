namespace Intech.Invoice;

sealed class ConstInvoice : Invoice
{
    readonly Invoice origin;
    readonly string clientName;
    readonly string number;
    readonly DateOnly date;
    readonly DateOnly dueDate;
    readonly Money subtotal;
    readonly Money vatAmount;
    readonly Money total;
    readonly string state;

    public ConstInvoice(Invoice origin, string clientName, string number, DateOnly date, DateOnly dueDate, Money subtotal, Money vatAmount, Money total, string state)
    {
        this.origin = origin;
        this.clientName = clientName;
        this.number = number;
        this.date = date;
        this.dueDate = dueDate;
        this.subtotal = subtotal;
        this.vatAmount = vatAmount;
        this.total = total;
        this.state = state;
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
                    .With("State", state);
    }
}