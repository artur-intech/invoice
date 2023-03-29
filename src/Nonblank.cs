namespace Intech.Invoice;

sealed class Nonblank
{
    readonly UserInput origin;

    public Nonblank(UserInput origin)
    {
        this.origin = origin;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(origin.ToString()) || string.IsNullOrWhiteSpace(origin.ToString()))
        {
            throw new Exception("Value cannot be empty.");
        }

        return origin.ToString();
    }
}