namespace Intech.Invoice;

sealed class DelimitedInvoiceList
{
    readonly IEnumerable<Invoice> invoices;

    public DelimitedInvoiceList(IEnumerable<Invoice> invoices)
    {
        this.invoices = invoices;
    }

    public void Print()
    {
        Console.WriteLine($"Records total: {invoices.ToList().Count}");

        foreach (var invoice in invoices)
        {
            Console.WriteLine(Delimiter());

            invoice.WithDetails((int id, string clientName, string number, DateOnly date, DateOnly dueDate,
            long subtotal, long vatAmount, long total, bool paid, DateOnly? paidDate) =>
            {
                Console.WriteLine($"Id: {id}");
                Console.WriteLine($"Client: {clientName}");
                Console.WriteLine($"Number: {number}");
                Console.WriteLine($"Date: {date}");
                Console.WriteLine($"Due date: {dueDate}");
                Console.WriteLine($"Total: {total}");
                Console.WriteLine($"Paid: {paid}");
            });
        }
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
