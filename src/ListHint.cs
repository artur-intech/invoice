namespace Intech.Invoice;

sealed class ListHint<T>
{
    readonly IEnumerable<T> list;

    public ListHint(IEnumerable<T> list)
    {
        this.list = list;
    }

    public override string ToString()
    {
        return $" ({string.Join(", ", list.Select((listItem) =>
        {
            dynamic item = listItem;

            return
            $"""
            {item.Id()} - "{item}"
            """;
        }))})";
    }
}