namespace Intech.Invoice;

sealed class ReverseChargedVatRate : VatRate
{
    public static bool operator ==(ReverseChargedVatRate a, ReverseChargedVatRate b)
    {
        if (ReferenceEquals(a, b))
            return true;

        return true;
    }

    public static bool operator !=(ReverseChargedVatRate a, ReverseChargedVatRate b)
    {
        return !(a == b);
    }

    public int IntValue()
    {
        return 0;
    }

    public int VatAmount(int amountWithoutVat)
    {
        return 0;
    }

    public override bool Equals(object? obj)
    {
        return obj is ReverseChargedVatRate other && GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IntValue());
    }
}
