namespace Intech.Invoice;

interface VatRate
{
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
        var standardRate = int.Parse(Environment.GetEnvironmentVariable("STANDARD_VAT_RATE"));
        return new DefaultVatRate(standardRate);
    }

    int IntValue();
    int VatAmount(int amountWithoutVat);
}
