namespace Intech.Invoice;

sealed class Money
{
    readonly long number;

    public Money(long number)
    {
        this.number = number;
    }

    public override string ToString()
    {
        return $"{number} EUR";
    }
}