using System.Dynamic;
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
        var skipFixtures = (skipFixturesProperty is not null) && bool.Parse(skipFixturesProperty.ToString());
        if (skipFixtures) return;

        CreateSupplierFixtures();
        CreateClientFixtures();

        dynamic firstInvoiceFixture = CreateInvoiceFixture();
        dynamic secondInvoiceFixture = CreateInvoiceFixture();

        CreateLineItemFixture(firstInvoiceFixture.Id);
        CreateLineItemFixture(secondInvoiceFixture.Id);

        fixtures.Add("invoices", new Dictionary<string, object> { { "one", InvoiceFixture(firstInvoiceFixture.Id) },
                                                                  { "two", InvoiceFixture(secondInvoiceFixture.Id) } });
    }

    [SetUp]
    protected void SaveOriginalEnvValues()
    {
        originalEnvVatRate = Environment.GetEnvironmentVariable("STANDARD_VAT_RATE");
    }

    protected void CreateSupplierFixtures()
    {
        var first = CreateSupplierFixture();
        var second = CreateSupplierFixture();

        fixtures.Add("suppliers", new Dictionary<string, object> { { "one", first }, { "two", second } });
    }

    protected void CreateClientFixtures()
    {
        var first = CreateClientFixture();
        var second = CreateClientFixture();

        fixtures.Add("clients", new Dictionary<string, object> { { "one", first }, { "two", second } });
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

    protected Migration Migration(string id = "test", string sql = "whatever")
    {
        var path = Path.Combine(migrationsPath, $"{id}.pgsql");
        File.WriteAllText(path, sql);

        return new FileMigration(path, pgDataSource);
    }

    protected ExpandoObject CreateInvoiceFixture()
    {
        return CreateInvoiceFixture(ValidDate());
    }

    protected ExpandoObject CreateInvoiceFixture(DateOnly date)
    {
        var pgInvoices = new PgInvoices(pgDataSource);
        var pgInvoice = pgInvoices.Add(number: ValidNumber(),
            date: date,
            dueDate: date,
            vatRate: ValidVatRate(),
            supplierId: ValidSupplierId(),
            clientId: ValidClientId());

        return InvoiceFixture(pgInvoice.Id());
    }

    protected ExpandoObject InvoiceFixture(int id)
    {
        DateOnly? PaidDate(NpgsqlDataReader reader)
        {
            return !reader.IsDBNull(reader.GetOrdinal("paid_date")) ? Date(reader, "paid_date") : null;
        }

        DateOnly Date(NpgsqlDataReader reader, string column)
        {
            return reader.GetFieldValue<DateOnly>(reader.GetOrdinal(column));
        }

        var sql = $"""
            SELECT
            invoices.*,
            COALESCE(SUM(price * quantity::int), 0) AS subtotal,
            COALESCE((SUM(price * quantity::int) * vat_rate) / 100, 0) AS vat_amount,
            COALESCE(SUM(price * quantity::int) + ((SUM(price * quantity::int) * vat_rate) / 100), 0) AS total
            FROM
            invoices
            LEFT JOIN
            line_items ON invoices.id = line_items.invoice_id
            WHERE
            invoices.id = {id}
            GROUP BY
            invoices.id
            """;
        using var cmd = pgDataSource.CreateCommand(sql);
        using var reader = cmd.ExecuteReader();
        reader.Read();

        dynamic invoice = new ExpandoObject();
        invoice.Id = id;
        invoice.Number = reader["number"];
        invoice.Date = Date(reader, "date");
        invoice.DueDate = Date(reader, "due_date");
        invoice.VatRate = reader["vat_rate"];
        invoice.ClientId = reader["client_id"];
        invoice.SupplierName = reader["supplier_name"];
        invoice.SupplierAddress = reader["supplier_address"];
        invoice.SupplierVatNumber = reader["supplier_vat_number"];
        invoice.SupplierIban = reader["supplier_iban"];
        invoice.ClientName = reader["client_name"];
        invoice.ClientAddress = reader["client_address"];
        invoice.ClientVatNumber = reader["client_vat_number"];
        invoice.Paid = reader["paid"];
        invoice.Subtotal = (long)reader["subtotal"];
        invoice.VatAmount = (long)reader["vat_amount"];
        invoice.Total = (long)reader["total"];
        invoice.PaidDate = PaidDate(reader);

        return invoice;
    }

    protected ExpandoObject CreateLineItemFixture(int invoiceId)
    {
        var pgLineItems = new PgLineItems(pgDataSource);
        var pgLineItemId = pgLineItems.Add(invoiceId: invoiceId, name: "test", price: 100, quantity: 1);

        using var cmd = pgDataSource.CreateCommand($"SELECT * FROM line_items WHERE id = {pgLineItemId}");
        using var reader = cmd.ExecuteReader();
        reader.Read();

        dynamic lineItem = new ExpandoObject();
        lineItem.Name = reader["name"];
        lineItem.Price = reader["price"];
        lineItem.Quantity = reader["quantity"];

        return lineItem;
    }

    protected ExpandoObject CreateSupplierFixture()
    {
        var pgSuppliers = new PgSuppliers(pgDataSource);
        var pgSupplier = pgSuppliers.Add(ValidName(), ValidAddress(), ValidVatNumber(), ValidIban());

        return SupplierFixture(pgSupplier.Id());
    }

    protected ExpandoObject SupplierFixture(int id)
    {
        using var cmd = pgDataSource.CreateCommand($"SELECT * FROM suppliers WHERE id = {id}");
        using var reader = cmd.ExecuteReader();
        reader.Read();

        dynamic supplier = new ExpandoObject();
        supplier.Id = id;
        supplier.Name = reader["name"];
        supplier.Address = reader["address"];
        supplier.VatNumber = reader["vat_number"];
        supplier.Iban = reader["iban"];

        return supplier;
    }

    protected ExpandoObject CreateClientFixture()
    {
        var pgClients = new PgClients(pgDataSource);
        var pgClient = pgClients.Add(ValidName(), ValidAddress(), ValidVatNumber());

        return ClientFixture(pgClient.Id());
    }

    protected ExpandoObject ClientFixture(int id)
    {
        using var cmd = pgDataSource.CreateCommand($"SELECT * FROM clients WHERE id = {id}");
        using var reader = cmd.ExecuteReader();
        reader.Read();

        dynamic client = new ExpandoObject();
        client.Id = id;
        client.Name = reader["name"];
        client.Address = reader["address"];
        client.VatNumber = reader["vat_number"];

        return client;
    }

    protected string ValidName()
    {
        return new Random().Next().ToString();
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
        // Proper validation is not relevant, so this is considered valid.
        return "DE12a";
    }

    protected string ValidNumber()
    {
        return new Random().Next().ToString();
    }

    protected int ValidVatRate()
    {
        return 20;
    }

    protected int ValidSupplierId()
    {
        dynamic supplier = fixtures["suppliers"]["one"];
        return supplier.Id;
    }

    protected int ValidClientId()
    {
        dynamic client = fixtures["clients"]["one"];
        return client.Id;
    }

    protected DateOnly ValidDate()
    {
        return new DateOnly(1970, 01, 01);
    }
}
