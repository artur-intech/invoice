using System.Text.RegularExpressions;

namespace Intech.Invoice;

sealed class StrictInputEmail : UserInput
{
    readonly UserInput origin;

    public StrictInputEmail(UserInput origin)
    {
        this.origin = origin;
    }

    public override string ToString()
    {
        if (FormatInvalid()) throw new Exception("Email has invalid format.");

        return origin.ToString();
    }

    bool FormatInvalid()
    {
        return !Regex.IsMatch(origin.ToString(), Pattern(), RegexOptions.IgnoreCase);
    }

    string Pattern()
    {
        return "^.+@.+$";
    }
}
