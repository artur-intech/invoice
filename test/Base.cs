using Npgsql;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class Base
{
    protected NpgsqlDataSource? pgDataSource;
    protected Dictionary<string, Dictionary<string, object>>? fixtures;

    public Base()
    {
        fixtures = new Dictionary<string, Dictionary<string, object>>();
    }

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

        var invoiceId = invoices.Create(firstSupplier.Id, firstClient.Id);
        var lineItem = new LineItemFixtures(pgDataSource).Create(invoiceId);

        var invoice = invoices.Fetch(invoiceId);

        fixtures = new Dictionary<string, Dictionary<string, object>>
        {
            { "suppliers", new Dictionary<string, object> { { "one", firstSupplier }, { "two", secondSupplier } } },
            { "clients", new Dictionary<string, object> { { "one", firstClient }, { "two", secondClient } } },
            { "invoices", new Dictionary<string, object> { { "one", invoice } } },
            { "line_items", new Dictionary<string, object> { { "one", lineItem } } }
        };
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
        pgDataSource.CreateCommand("TRUNCATE suppliers, clients, invoices, line_items RESTART IDENTITY CASCADE").ExecuteNonQuery();
    }

    [OneTimeTearDown]
    protected void DisposePgDataSource()
    {
        pgDataSource.Dispose();
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
}