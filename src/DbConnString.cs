namespace Intech.Invoice;

class DbConnString
{
    readonly string host;
    readonly string user;
    readonly string password;
    readonly string db;

    public DbConnString(string host, string user, string password, string db)
    {
        this.host = host;
        this.user = user;
        this.password = password;
        this.db = db;
    }

    public string Npgsql()
    {
        return $"Server={host}; User Id={user}; Password={password}; Database={db}";
    }

    public string PgDump()
    {
        return $"postgres://{host}:{password}@{host}/{db}";
    }
}