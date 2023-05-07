namespace Intech.Invoice;

sealed class ConstSupplier : Supplier
{
    readonly IDictionary<string, object> rawDbData;

    public ConstSupplier(IDictionary<string, object> rawDbData)
    {
        this.rawDbData = rawDbData;
    }

    public int Id()
    {
        return (int)rawDbData["id"];
    }

    public string Name()
    {
        return (string)rawDbData["name"];
    }

    public override string ToString()
    {
        return Name();
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
        callback.Invoke(Id(), Name(), Address(), VatNumber(), Iban(), Email());
    }

    string Address()
    {
        return (string)rawDbData["address"];
    }

    string VatNumber()
    {
        return (string)rawDbData["vat_number"];
    }

    string Iban()
    {
        return (string)rawDbData["iban"];
    }

    string Email()
    {
        return (string)rawDbData["email"];
    }
}
