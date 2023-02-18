namespace Intech.Invoice;

sealed class ConsoleInvoiceList
{
    readonly IEnumerable<Invoice> invoices;

    public ConsoleInvoiceList(IEnumerable<Invoice> invoices)
    {
        this.invoices = invoices;
    }

    public void Print()
    {
        foreach (var invoice in invoices)
        {
            Console.Write(invoice.Print(new ConsoleMedia()).Text());
        }
    }
}