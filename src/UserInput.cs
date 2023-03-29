namespace Intech.Invoice;

sealed class UserInput
{
    readonly string str;

    public UserInput(string str)
    {
        this.str = str;
    }

    public override string ToString()
    {
        return str;
    }
}
