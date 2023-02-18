namespace Intech.Invoice;

sealed class DefaultVatRate : VatRate
{
    readonly int value;

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
}