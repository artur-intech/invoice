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
        var i = 0;
        var delimiter = new string('-', 50);

        foreach (var invoice in invoices)
        {
            if (i != 0)
            {
                Console.Write(Environment.NewLine + delimiter + Environment.NewLine);
            }

            Console.Write(invoice.Print(new ConsoleMedia()).Text());
            i++;
        }
    }
}