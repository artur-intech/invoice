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
        var i = 0;

        foreach (dynamic listItem in list)
        {
            if (i != 0)
            {
                Console.Write(Environment.NewLine + Delimiter() + Environment.NewLine);
            }

            Console.Write(listItem.Print(new ConsoleMedia()).Text());
            i++;
        }
    }

    string Delimiter()
    {
        return new string('-', 50);
    }
}