namespace Intech.Invoice;

sealed class SupplierHint
{
    readonly Suppliers suppliers;

    public SupplierHint(Suppliers suppliers)
    {
        this.suppliers = suppliers;
    }

    public override string ToString()
    {
        return $" ({string.Join(", ", suppliers.Select((supplier) =>
        {
            return
            $"""
            {supplier.Id()} - "{supplier}"
            """;
        }))})";
    }
}