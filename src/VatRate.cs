namespace Intech.Invoice;

interface VatRate
{
    const int standardRate = 20;

    class Fake : VatRate
    {
        readonly int value;

        public Fake(int value)
        {
            this.value = value;
        }

        int VatRate.IntValue()
        {
            return value;
        }

        int VatRate.VatAmount(int amountWithoutVat)
        {
            throw new NotImplementedException();
        }
    }

    public static VatRate Standard()
    {
        return new DefaultVatRate(standardRate);
    }

    int IntValue();
    int VatAmount(int amountWithoutVat);
}