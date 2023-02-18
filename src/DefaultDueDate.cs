namespace Intech.Invoice;

sealed class DefaultDueDate : DueDate
{
    readonly int days;
    readonly DateOnly startDate;

    public DefaultDueDate(int days, DateOnly startDate)
    {
        this.days = days;
        this.startDate = startDate;
    }

    public DateOnly Date()
    {
        return startDate.AddDays(days);
    }
}