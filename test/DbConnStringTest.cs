using NUnit.Framework;

namespace Intech.Invoice.Test;

class DbConnStringTest
{
    [Test]
    public void ReturnsNpgsqlFormat()
    {
        var connString = new DbConnString(ValidHost(), ValidUser(), ValidPassword(), ValidDb());
        var expected = connString.Npgsql();

        Assert.AreEqual($"Server={ValidHost()}; User Id={ValidUser()}; Password={ValidPassword()}; Database={ValidDb()}", expected);
    }

    [Test]
    public void ReturnsPgdumpFormat()
    {
        var connString = new DbConnString(ValidHost(), ValidUser(), ValidPassword(), ValidDb());
        var expected = connString.PgDump();

        Assert.AreEqual($"postgres://{ValidUser()}:{ValidPassword()}@{ValidHost()}/{ValidDb()}", expected);
    }

    string ValidHost() => "host";
    string ValidUser() => "user";
    string ValidPassword() => "password";
    string ValidDb() => "db";
}