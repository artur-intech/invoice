namespace Intech.Invoice;

sealed class InterpolatedEmailTemplate : EmailTemplate
{
    readonly EmailTemplate origin;
    readonly DateOnly dueDate;
    readonly long total;
    readonly string clientName;
    readonly string supplierName;

    public InterpolatedEmailTemplate(EmailTemplate origin, DateOnly dueDate, long total, string clientName, string supplierName)
    {
        this.origin = origin;
        this.dueDate = dueDate;
        this.total = total;
        this.clientName = clientName;
        this.supplierName = supplierName;
    }

    public override string ToString()
    {
        return string.Format(origin.ToString(), clientName, total, dueDate, supplierName);
    }
}
