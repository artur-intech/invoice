namespace Intech.Invoice;

interface DueDate
{
    const int standardDays = 10;

    class Fake : DueDate
    {
        readonly DateOnly date;

        public Fake(DateOnly date)
        {
            this.date = date;
        }

        DateOnly DueDate.Date()
        {
            return date;
        }
    }

    public static DueDate Standard(DateOnly startDate)
    {
        return new DefaultDueDate(standardDays, startDate);
    }

    DateOnly Date();
}
