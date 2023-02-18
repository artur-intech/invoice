namespace Intech.Invoice.Test;

readonly struct SupplierFixture
{
    public readonly int Id { get; init; }
    public readonly string Name { get; init; }
    public readonly string Address { get; init; }
    public readonly string VatNumber { get; init; }
    public readonly string Iban { get; init; }
}