namespace Intech.Invoice;

interface Invoice
{
    int Id();
    string ToString();
    ConsoleMedia Print(ConsoleMedia media);
}