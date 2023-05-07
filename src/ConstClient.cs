namespace Intech.Invoice;

sealed class ConstClient : Client
{
    readonly IDictionary<string, object> rawDbData;

    public ConstClient(IDictionary<string, object> rawDbData)
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

    public void Modify(string newName, string newAddress, string newVatNumber)
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Name();
    }

    public void WithDetails(Action<int, string, string, string, string> callback)
    {
        callback.Invoke(Id(), Name(), Address(), VatNumber(), Email());
    }

    string Address()
    {
        return (string)rawDbData["address"];
    }

    string VatNumber()
    {
        return (string)rawDbData["vat_number"];
    }

    string Email()
    {
        return (string)rawDbData["email"];
    }
}
