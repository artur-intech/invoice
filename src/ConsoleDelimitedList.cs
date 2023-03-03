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
        var delimiter = new string('-', 50);

        foreach (dynamic listItem in list)
        {
            if (i != 0)
            {
                Console.Write(Environment.NewLine + delimiter + Environment.NewLine);
            }

            Console.Write(listItem.Print(new ConsoleMedia()).Text());
            i++;
        }
    }
}