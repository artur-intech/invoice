using System.Text.RegularExpressions;

namespace Intech.Invoice;

sealed class StrictInputIban : UserInput
{
    readonly UserInput origin;

    public StrictInputIban(UserInput origin)
    {
        this.origin = origin;
    }

    public override string ToString()
    {
        if (FormatInvalid()) throw new Exception("IBAN has invalid format.");

        return origin.ToString();
    }

    bool FormatInvalid()
    {
        return !Regex.IsMatch(origin.ToString(), Pattern(), RegexOptions.IgnoreCase);
    }

    string Pattern()
    {
        return "^([a-z]{2})([0-9]{2})([a-z0-9]{1,30})$";
    }
}