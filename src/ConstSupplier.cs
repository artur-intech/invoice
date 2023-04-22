namespace Intech.Invoice;

sealed class ConstSupplier : Supplier
{
    readonly Supplier origin;
    readonly string name;
    readonly string address;
    readonly string vatNumber;
    readonly string iban;
    readonly string email;

    public ConstSupplier(Supplier origin, string name, string address, string vatNumber, string iban, string email)
    {
        this.origin = origin;
        this.name = name;
        this.address = address;
        this.vatNumber = vatNumber;
        this.iban = iban;
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

    public void Modify(string newName, string newAddress, string newVatNumber, string newIban, string newEmail)
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public void WithDetails(Action<int, string, string, string, string, string> callback)
    {
        callback.Invoke(Id(), Name(), address, vatNumber, iban, email);
    }
}
