namespace Intech.Invoice;

interface LineItems
{
    void Add(int invoiceId, string name, int price, int quantity);
}