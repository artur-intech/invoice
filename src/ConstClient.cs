using System.Data;

namespace Intech.Invoice;

sealed class ConstClient : Client
{
    readonly IDataReader reader;

    public ConstClient(IDataReader reader)
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

    public ConsoleMedia Print(ConsoleMedia media)
    {
        return media.With("Id", Id())
                    .With("Name", Name())
                    .With("Address", Address())
                    .With("VAT number", VatNumber())
                    .With("Email", Email());
    }

    string Address()
    {
        return (string)reader["address"];
    }

    string VatNumber()
    {
        return (string)reader["vat_number"];
    }

    string Email()
    {
        return (string)reader["email"];
    }
}
