namespace Intech.Invoice;

sealed class ConstClient : Client
{
    readonly Client origin;
    readonly string name;
    readonly string address;
    readonly string vatNumber;
    readonly string email;

    public ConstClient(Client origin, string name, string address, string vatNumber, string email)
    {
        this.origin = origin;
        this.name = name;
        this.address = address;
        this.vatNumber = vatNumber;
        this.email = email;
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
                    .With("Email", email);
    }

    public void Modify(string newName, string newAddress, string newVatNumber)
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }
}
