using System.Text.RegularExpressions;

namespace Intech.Invoice;

sealed class StrictInputVatNumber : UserInput
{
    readonly UserInput origin;

    public StrictInputVatNumber(UserInput origin)
    {
        this.origin = origin;
    }

    public override string ToString()
    {
        if (FormatInvalid()) throw new Exception("VAT number has invalid format.");

        return origin.ToString();
    }

    bool FormatInvalid()
    {
        return !Regex.IsMatch(origin.ToString(), Pattern(), RegexOptions.IgnoreCase);
    }

    string Pattern()
    {
        return "^([a-z]{2})([a-z0-9]{2,13})$";
    }
}
