namespace Intech.Invoice;

// TODO Implement VatRate identity
sealed class ReverseChargedVatRate : VatRate
{
    public int IntValue()
    {
        return 0;
    }

    public int VatAmount(int amountWithoutVat)
    {
        return 0;
    }
}