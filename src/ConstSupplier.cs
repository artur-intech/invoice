using System.Data;

namespace Intech.Invoice;

sealed class ConstSupplier : Supplier
{
    readonly IDataReader reader;

    public ConstSupplier(IDataReader reader)
    {
        this.reader = reader;
    }

    public int Id()
    {
        return (int)reader["id"];
    }

    public string Name()
    {
        return (string)reader["name"];
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
        return (string)reader["address"];
    }

    string VatNumber()
    {
        return (string)reader["vat_number"];
    }

    string Iban()
    {
        return (string)reader["iban"];
    }

    string Email()
    {
        return (string)reader["email"];
    }
}
