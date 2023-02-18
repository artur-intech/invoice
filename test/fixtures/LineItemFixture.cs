namespace Intech.Invoice.Test;

readonly struct LineItemFixture
{
    public readonly int Id { get; init; }
    public readonly string Name { get; init; }
    public readonly int Price { get; init; }
    public readonly int Quantity { get; init; }
}