using NUnit.Framework;

namespace Intech.Invoice.Test;

class DbConnStringTest
{
    [Test]
    public void ReturnsNpgsqlFormat()
    {
        var connString = new DbConnString(ValidHost(), ValidUser(), ValidPassword(), ValidDb());
        Assert.AreEqual($"Server={ValidHost()}; User Id={ValidUser()}; Password={ValidPassword()}; Database={ValidDb()}", connString.Npgsql());
    }

    [Test]
    public void ReturnsPgdumpFormat()
    {
        var connString = new DbConnString(ValidHost(), ValidUser(), ValidPassword(), ValidDb());
        Assert.AreEqual($"postgres://{ValidUser()}:{ValidPassword()}@{ValidHost()}/{ValidDb()}", connString.PgDump());
    }

    string ValidHost() => "host";
    string ValidUser() => "user";
    string ValidPassword() => "password";
    string ValidDb() => "db";
}
