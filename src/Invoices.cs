namespace Intech.Invoice;

interface Invoices : IEnumerable<Invoice>
{
    Invoice Add(string number, DateOnly date, DateOnly dueDate, int vatRate, int supplierId, int customerId, DateOnly? paidDate);
}
