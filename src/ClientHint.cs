namespace Intech.Invoice;

sealed class ClientHint
{
    readonly Clients clients;

    public ClientHint(Clients clients)
    {
        this.clients = clients;
    }

    public override string ToString()
    {
        return $" ({string.Join(", ", clients.Select((client) =>
        {
            return
            $"""
            {client.Id()} - "{client}"
            """;
        }))})";
    }
}