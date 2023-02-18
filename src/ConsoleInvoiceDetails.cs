namespace Intech.Invoice;

sealed class ConsoleInvoiceDetails
{
    readonly Invoice invoice;
    readonly ConsoleMedia media;

    public ConsoleInvoiceDetails(Invoice invoice, ConsoleMedia media)
    {
        this.invoice = invoice;
        this.media = media;
    }

    public void Print()
    {
        Console.Write(invoice.Print(media).Text());
    }
}