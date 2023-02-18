namespace Intech.Invoice;

interface Invoices
{
    Invoice Add(string number, DateOnly date, DateOnly dueDate, int vatRate, int supplierId, int customerId);
    IEnumerable<PgInvoice> Fetch();
}