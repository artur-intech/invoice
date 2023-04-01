namespace Intech.Invoice;

sealed class ConsoleDelimitedList<T>
{
    readonly IEnumerable<T> list;

    public ConsoleDelimitedList(IEnumerable<T> list)
    {
        this.list = list;
    }

    public void Print()
    {
        Console.WriteLine($"Records total: {list.ToList().Count}");

        foreach (dynamic listItem in list)
        {
            Console.Write(Delimiter() + Environment.NewLine);
            Console.WriteLine(listItem.Print(new ConsoleMedia()).Text());
        }
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}
