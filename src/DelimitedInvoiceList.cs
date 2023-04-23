namespace Intech.Invoice;

sealed class DelimitedInvoiceList<T>
{
    readonly IEnumerable<T> list;

    public DelimitedInvoiceList(IEnumerable<T> list)
    {
        this.list = list;
    }

    public void Print()
    {
        Console.WriteLine($"Records total: {list.ToList().Count}");

        foreach (dynamic listItem in list)
        {
            Console.WriteLine(Delimiter());

            listItem.WithDetails((Action<int, string, string, DateOnly, DateOnly, long, long, long, bool, DateOnly?>)
            ((int id, string clientName, string number, DateOnly date, DateOnly dueDate, long subtotal, long vatAmount,
            long total, bool paid, DateOnly? paidDate) =>
            {
                Console.WriteLine($"Id: {id}");
                Console.WriteLine($"Client: {clientName}");
                Console.WriteLine($"Number: {number}");
                Console.WriteLine($"Date: {date}");
                Console.WriteLine($"Due date: {dueDate}");
                Console.WriteLine($"Total: {total}");
                Console.WriteLine($"Paid: {paid}");
            }));
        }
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
