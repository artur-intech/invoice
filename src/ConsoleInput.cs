namespace Intech.Invoice;

sealed class ConsoleInput : UserInput
{
    readonly string value;

    public ConsoleInput(string value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value;
    }
}
