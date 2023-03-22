using Intech.Invoice.DbMigration;
using Npgsql;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class Base
{
    protected NpgsqlDataSource? pgDataSource;
    protected Dictionary<string, Dictionary<string, object>>? fixtures;
    protected string? originalEnvVatRate;
    protected string? migrationsPath;

    [OneTimeSetUp]
    protected void SetupPgDataSource()
    {
        var pgHost = Environment.GetEnvironmentVariable("PG_HOST");
        var pgUser = Environment.GetEnvironmentVariable("PG_USER");
        var pgPassword = Environment.GetEnvironmentVariable("PG_PASSWORD");
        var pgDatabase = Environment.GetEnvironmentVariable("PG_DATABASE");
        var dbConnectionString = $"Server={pgHost}; User Id={pgUser}; Password={pgPassword}; Database={pgDatabase}; Include Error Detail=true";
        pgDataSource = NpgsqlDataSource.Create(dbConnectionString);
    }

    [SetUp]
    protected void InitFixtures()
    {
        fixtures = new Dictionary<string, Dictionary<string, object>>();
    }

    [SetUp]
    protected void CreateFixtures()
    {
        object? skipFixturesProperty = TestContext.CurrentContext.Test.Properties.Get("SkipFixtureCreation");
        var skipFixtures = (skipFixturesProperty is not null) ? bool.Parse(skipFixturesProperty.ToString()) : false;
        if (skipFixtures) return;

        var firstSupplier = new SupplierFixtures(pgDataSource).Create();
        var secondSupplier = new SupplierFixtures(pgDataSource).Create(name: "best supplier");
        var firstClient = new ClientFixtures(pgDataSource).Create();
        var secondClient = new ClientFixtures(pgDataSource).Create(name: "best client");
        var invoices = new InvoiceFixtures(pgDataSource);

        var invoiceId = invoices.Create(firstSupplier.Id, firstClient.Id, date: new DateOnly(1970, 01, 01), dueDate: new DateOnly(1970, 01, 01));
        var lineItem = new LineItemFixtures(pgDataSource).Create(invoiceId);

        var firstInvoice = invoices.Fetch(invoiceId);
        var secondInvoiceId = invoices.Create(firstSupplier.Id, firstClient.Id, date: new DateOnly(1970, 01, 02), dueDate: new DateOnly(1970, 01, 02), paidDate: new DateOnly(1970, 01, 03));
        new LineItemFixtures(pgDataSource).Create(secondInvoiceId);
        var secondInvoice = invoices.Fetch(secondInvoiceId);

        fixtures = new Dictionary<string, Dictionary<string, object>>
        {
            { "suppliers", new Dictionary<string, object> { { "one", firstSupplier }, { "two", secondSupplier } } },
            { "clients", new Dictionary<string, object> { { "one", firstClient }, { "two", secondClient } } },
            { "invoices", new Dictionary<string, object> { { "one", firstInvoice }, { "two", secondInvoice } } },
            { "line_items", new Dictionary<string, object> { { "one", lineItem } } }
        };
    }

    [SetUp]
    protected void SaveOriginalEnvValues()
    {
        originalEnvVatRate = Environment.GetEnvironmentVariable("STANDARD_VAT_RATE");
    }

    // TODO Remove duplication
    protected void CreateSupplierFixtures()
    {
        var firstFixture = new SupplierFixtures(pgDataSource).Create();
        var secondFixture = new SupplierFixtures(pgDataSource).Create(name: "best supplier");
        fixtures.Add("suppliers", new Dictionary<string, object> { { "one", firstFixture }, { "two", secondFixture } });
    }

    // TODO Remove duplication
    protected void CreateClientFixtures()
    {
        var firstFixture = new ClientFixtures(pgDataSource).Create();
        var secondFixture = new ClientFixtures(pgDataSource).Create(name: "best client");
        fixtures.Add("clients", new Dictionary<string, object> { { "one", firstFixture }, { "two", secondFixture } });
    }

    [TearDown]
    protected void CleanUpDb()
    {
        pgDataSource.CreateCommand("TRUNCATE suppliers, clients, invoices, line_items, applied_migrations RESTART IDENTITY CASCADE").ExecuteNonQuery();
    }

    [TearDown]
    protected void RestoreOriginalEnvValues()
    {
        Environment.SetEnvironmentVariable("STANDARD_VAT_RATE", originalEnvVatRate);
    }

    [OneTimeTearDown]
    protected void DisposePgDataSource()
    {
        pgDataSource.Dispose();
    }

    [SetUp]
    protected void SetUp()
    {
        migrationsPath = Path.Combine(Path.GetTempPath(), new Random().Next().ToString());
    }

    protected void RunApp(string[]? arguments = default)
    {
        var entryPoint = typeof(Program).Assembly.EntryPoint!;
        entryPoint.Invoke(null, new object[] { arguments ?? Array.Empty<string>() });
    }

    protected string CapturedStdOut(Action callback)
    {
        TextWriter originalStdOut = Console.Out;

        using var newStdOut = new StringWriter();
        Console.SetOut(newStdOut);

        callback.Invoke();
        var capturedOutput = newStdOut.ToString();

        Console.SetOut(originalStdOut);

        return capturedOutput;
    }

    protected void SubstituteStdIn(string content, Action callback)
    {
        TextReader originalStdIn = Console.In;

        using var newStdIn = new StringReader(content);
        Console.SetIn(newStdIn);

        callback.Invoke();

        Console.SetIn(originalStdIn);
    }

    protected string ValidName()
    {
        return "name";
    }

    protected string ValidAddress()
    {
        return "address";
    }

    protected string ValidVatNumber()
    {
        return "vat_number";
    }

    protected string ValidIban()
    {
        return "iban";
    }

    protected Migration Migration(string id = "test", string sql = "whatever")
    {
        var path = Path.Combine(migrationsPath, $"{id}.pgsql");
        File.WriteAllText(path, sql);

        return new FileMigration(path, pgDataSource);
    }
}