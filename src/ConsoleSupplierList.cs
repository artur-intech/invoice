namespace Intech.Invoice;

sealed class ConsoleSupplierList
{
    readonly IEnumerable<Supplier> suppliers;

    public ConsoleSupplierList(IEnumerable<Supplier> suppliers)
    {
        this.suppliers = suppliers;
    }

    public void Print()
    {
        var i = 0;
        var delimiter = new string('-', 50);

        foreach (var supplier in suppliers)
        {
            if (i != 0)
            {
                Console.Write(Environment.NewLine + delimiter + Environment.NewLine);
            }

            Console.Write(supplier.Print(new ConsoleMedia()).Text());
            i++;
        }
    }
}