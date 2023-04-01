namespace Intech.Invoice;

sealed class ConstSupplier : Supplier
{
    readonly Supplier origin;
    readonly string name;
    readonly string address;
    readonly string vatNumber;
    readonly string iban;

    public ConstSupplier(Supplier origin, string name, string address, string vatNumber, string iban)
    {
        this.origin = origin;
        this.name = name;
        this.address = address;
        this.vatNumber = vatNumber;
        this.iban = iban;
    }

    public int Id()
    {
        return origin.Id();
    }

    public string Name()
    {
        return name;
    }

    public override string ToString()
    {
        return origin.ToString();
    }

    public ConsoleMedia Print(ConsoleMedia media)
    {
        return media.With("Id", origin.Id())
                    .With("Name", name)
                    .With("Address", address)
                    .With("VAT number", vatNumber)
                    .With("IBAN", iban);
    }

    public void Modify(string newName, string newAddress, string newVatNumber, string newIban)
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }
}
