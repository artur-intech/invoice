namespace Intech.Invoice;

sealed class DefaultVatRate : VatRate
{
    readonly int value;

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

    public DefaultVatRate(int value)
    {
        this.value = value;
    }

    public int IntValue()
    {
        return value;
    }

    public int VatAmount(int amountWithoutVat)
    {
        return amountWithoutVat * value / 100;
    }

    public override bool Equals(object? obj)
    {
        return obj is DefaultVatRate other && value == other.value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(value);
    }
}