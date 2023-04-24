namespace Intech.Invoice;

sealed class InvoiceList : DelimitedList
{
    public InvoiceList(IEnumerable<Invoice> list) : base(list) { }

    protected override void PrintBody()
    {
        foreach (var invoice in list.Cast<Invoice>())
        {
            PrintDelimiter();

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
}
