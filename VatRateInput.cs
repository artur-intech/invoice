namespace Intech.Invoice;

sealed class VatRateInput
{
    readonly string value;

    public VatRateInput(string userInput)
    {
        value = userInput;
    }

    public VatRate VatRate()
    {
        if (value == "reverse-charged")
        {
            return new ReverseChargedVatRate();
        }
        else
        {
            int intValue;

            try
            {
                intValue = int.Parse(value);
            }
            catch (FormatException e)
            {
                throw new Exception("VAT rate must be a number", e);
            }

            if (int.IsNegative(intValue))
            {
                throw new Exception("VAT rate must be positive");
            }

            return new DefaultVatRate(intValue);
        }
    }
}