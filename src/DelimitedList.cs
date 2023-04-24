namespace Intech.Invoice;

abstract class DelimitedList
{
    readonly protected IEnumerable<object> list;

    protected abstract void PrintBody();

    public DelimitedList(IEnumerable<object> list)
    {
        this.list = list;
    }

    public void Print()
    {
        PrintHeader();
        PrintBody();
    }

    protected void PrintDelimiter()
    {
        Console.WriteLine(Delimiter());
    }

    void PrintHeader()
    {
        Console.WriteLine($"Records total: {list.ToList().Count}");
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
