namespace Intech.Invoice.Test;

readonly struct InvoiceFixture
{
    public readonly int Id { get; init; }
    public readonly string Number { get; init; }
    public readonly DateOnly Date { get; init; }
    public readonly DateOnly DueDate { get; init; }
    public readonly int VatRate { get; init; }
    public readonly long Subtotal { get; init; }
    public readonly long VatAmount { get; init; }
    public readonly long Total { get; init; }
    public readonly string SupplierName { get; init; }
    public readonly string State { get; init; }
}