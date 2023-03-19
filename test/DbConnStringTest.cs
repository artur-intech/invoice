using NUnit.Framework;

namespace Intech.Invoice.Test;

class DbConnStringTest
{
    [Test]
    public void ReturnsNpgsqlFormat()
    {
        var host = "host";
        var user = "user";
        var password = "password";
        var db = "db";

        var connString = new DbConnString(host, user, password, db);
        var expected = connString.Npgsql();

        Assert.AreEqual($"Server={host}; User Id={user}; Password={password}; Database={db}", expected);
    }

    [Test]
    public void ReturnsPgdumpFormat()
    {
        var host = "host";
        var user = "user";
        var password = "password";
        var db = "db";

        var connString = new DbConnString(host, user, password, db);
        var expected = connString.PgDump();

        Assert.AreEqual($"postgres://{host}:{password}@{host}/{db}", expected);
    }
}