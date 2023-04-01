namespace Intech.Invoice;

sealed class DefaultVatRate : VatRate
{
    readonly int number;

    public static bool operator ==(DefaultVatRate a, DefaultVatRate b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if ((object)a == null || (object)b == null)
            return false;

        return a.IntValue() == b.IntValue();
    }

    public static bool operator !=(DefaultVatRate a, DefaultVatRate b)
    {
        return !(a == b);
    }

    public DefaultVatRate(int number)
    {
        this.number = number;
    }

    public int IntValue()
    {
        return number;
    }

    public int VatAmount(int amountWithoutVat)
    {
        return amountWithoutVat * number / 100;
    }

    public override bool Equals(object? obj)
    {
        return obj is DefaultVatRate other && number == other.number;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(number);
    }
}
