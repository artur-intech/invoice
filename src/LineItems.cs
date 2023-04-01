namespace Intech.Invoice;

interface LineItems
{
    int Add(int invoiceId, string name, int price, int quantity);
}
