using System.Collections.Immutable;
using netDumbster.smtp;
using NUnit.Framework;

namespace Intech.Invoice.Test;

class ConsoleTest : Base
{
    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void CreatesSupplier()
    {
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar());

        var name = ValidName();
        var stdIn = $"""
            {name}
            {ValidAddress()}
            {ValidVatNumber()}
            {ValidIban()}
            {ValidEmail()}
            """;

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "supplier", "create" });
            });
        });

        Assert.AreEqual($"""
            Enter supplier name:
            Enter supplier address:
            Enter supplier VAT number:
            Enter supplier IBAN:
            Enter supplier email:
            Supplier {name} has been created.
            """, capturedStdOut);
        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar());
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void CreatesClient()
    {
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM clients").ExecuteScalar());

        var name = ValidName();
        var stdIn = $"""
            {name}
            {ValidAddress()}
            {ValidVatNumber()}
            {ValidEmail()}
            """;

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "client", "create" });
            });
        });

        Assert.AreEqual($"""
            Enter client name:
            Enter client address:
            Enter client VAT number:
            Enter client email:
            Client {name} has been created.
            """, capturedStdOut);
        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM clients").ExecuteScalar());
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void CreatesInvoice()
    {
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM invoices").ExecuteScalar());
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM line_items").ExecuteScalar());

        CreateSupplierFixtures();
        CreateClientFixtures();
        dynamic firstSupplier = fixtures["suppliers"]["one"];
        dynamic secondSupplier = fixtures["suppliers"]["two"];
        dynamic firstClient = fixtures["clients"]["one"];
        dynamic secondClient = fixtures["clients"]["two"];
        var stdIn = $"""
            {firstSupplier.Id}
            {firstClient.Id}
            {ValidVatRate()}
            Software development
            100
            160
            """;

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "invoice", "create" });
            });
        });

        var lastInvoiceId = (int)pgDataSource.CreateCommand("SELECT id FROM invoices ORDER BY id DESC LIMIT 1").ExecuteScalar();
        dynamic fixture = InvoiceFixture(lastInvoiceId);
        Assert.AreEqual($"""
            Enter supplier id ({firstSupplier.Id} - "{firstSupplier.Name}", {secondSupplier.Id} - "{secondSupplier.Name}"):
            Enter client id ({firstClient.Id} - "{firstClient.Name}", {secondClient.Id} - "{secondClient.Name}"):
            Enter VAT rate as positive integer, "reverse-charged" string or leave blank to apply the standard rate of {Environment.GetEnvironmentVariable("STANDARD_VAT_RATE")}%:
            Enter line item name:
            Enter line item price (integer):
            Enter line item quantity (integer):
            Invoice {fixture.Number} has been issued.
            """, capturedStdOut);
        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM invoices").ExecuteScalar());
        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM line_items").ExecuteScalar());
    }

    [Test]
    public void SavesInvoicePdf()
    {
        dynamic fixture = fixtures["invoices"]["one"];
        var invoicePdfFilePath = $"{fixture.SupplierName}_invoice_{fixture.Number}.pdf";
        FileAssert.DoesNotExist(invoicePdfFilePath);

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "pdf", fixture.Id.ToString() });
        });

        Assert.AreEqual("Invoice PDF has been saved.", capturedStdOut);
        FileAssert.Exists(invoicePdfFilePath);

        File.Delete(invoicePdfFilePath);
    }

    [Test]
    public void PrintsInvoiceDetails()
    {
        dynamic clientFixture = fixtures["clients"]["one"];
        dynamic fixture = fixtures["invoices"]["one"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "details", fixture.Id.ToString() });
        });

        Assert.AreEqual($"""
            Id: {fixture.Id}
            Client: {clientFixture.Name}
            Number: {fixture.Number}
            Date: {fixture.Date}
            Due date: {fixture.DueDate}
            Subtotal: {fixture.Subtotal}
            VAT amount: {fixture.VatAmount}
            Total: {fixture.Total}
            Paid: {fixture.Paid}
            Paid on: {Environment.NewLine}
            """, capturedStdOut);
    }
    [Test]
    public void PrintsAllInvoices()
    {
        dynamic clientFixture = fixtures["clients"]["one"];
        dynamic firstFixture = fixtures["invoices"]["one"];
        dynamic secondFixture = fixtures["invoices"]["two"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "list" });
        });

        Assert.AreEqual($"""
            Records total: {fixtures["invoices"].Count}
            {ListDelimiter()}
            Id: {secondFixture.Id}
            Client: {clientFixture.Name}
            Number: {secondFixture.Number}
            Date: {secondFixture.Date}
            Due date: {secondFixture.DueDate}
            Total: {secondFixture.Total}
            Paid: {secondFixture.Paid}
            {ListDelimiter()}
            Id: {firstFixture.Id}
            Client: {clientFixture.Name}
            Number: {firstFixture.Number}
            Date: {firstFixture.Date}
            Due date: {firstFixture.DueDate}
            Total: {firstFixture.Total}
            Paid: {firstFixture.Paid}{Environment.NewLine}
            """, capturedStdOut);
    }

    [Test]
    public void NoArguments()
    {
        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp();
        });

        Assert.AreEqual($"""
            Please provide one of the supported commands:
            {string.Join("\n", SupportedCommands())}.
            """, capturedStdOut);
    }

    [Test]
    public void InvalidArguments()
    {
        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invalid", "command" });
        });

        Assert.AreEqual($"""
            Please provide one of the supported commands:
            {string.Join("\n", SupportedCommands())}.
            """, capturedStdOut);
    }

    [Test]
    public void ModifiesSupplier()
    {
        dynamic fixture = fixtures["suppliers"]["one"];
        var newName = "new name";
        var newAddress = "new address";
        var newVatNumber = "new vat";
        var newIban = "new iban";
        var newEmail = "new email";
        Assert.AreNotEqual(newName, fixture.Name);
        Assert.AreNotEqual(newAddress, fixture.Address);
        Assert.AreNotEqual(newVatNumber, fixture.VatNumber);
        Assert.AreNotEqual(newIban, fixture.Iban);
        Assert.AreNotEqual(newEmail, fixture.Email);

        var stdIn = $"""
            {newName}
            {newAddress}
            {newVatNumber}
            {newIban}
            {newEmail}
            """;

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "supplier", "modify", fixture.Id.ToString() });
            });
        });

        Assert.AreEqual($"""
            Enter new supplier name:
            Enter new supplier address:
            Enter new supplier VAT number:
            Enter new supplier IBAN:
            Enter new supplier email:
            Supplier {newName} has been modified.
            """, capturedStdOut);
        fixture = SupplierFixture(fixture.Id);
        Assert.AreEqual(newName, fixture.Name);
    }

    [Test]
    public void PrintsAllSuppliers()
    {
        dynamic firstFixture = fixtures["suppliers"]["one"];
        dynamic secondFixture = fixtures["suppliers"]["two"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "supplier", "list" });
        });

        Assert.AreEqual($"""
            Records total: {fixtures["suppliers"].Count}
            {ListDelimiter()}
            Id: {firstFixture.Id}
            Name: {firstFixture.Name}
            Address: {firstFixture.Address}
            VAT number: {firstFixture.VatNumber}
            IBAN: {firstFixture.Iban}
            Email: {firstFixture.Email}
            {ListDelimiter()}
            Id: {secondFixture.Id}
            Name: {secondFixture.Name}
            Address: {secondFixture.Address}
            VAT number: {secondFixture.VatNumber}
            IBAN: {secondFixture.Iban}
            Email: {secondFixture.Email}{Environment.NewLine}
            """, capturedStdOut);
    }

    [Test]
    public void PrintsAllClients()
    {
        dynamic firstFixture = fixtures["clients"]["one"];
        dynamic secondFixture = fixtures["clients"]["two"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "client", "list" });
        });

        Assert.AreEqual($"""
            Records total: {fixtures["clients"].Count}
            {ListDelimiter()}
            Id: {firstFixture.Id}
            Name: {firstFixture.Name}
            Address: {firstFixture.Address}
            VAT number: {firstFixture.VatNumber}
            Email: {firstFixture.Email}
            {ListDelimiter()}
            Id: {secondFixture.Id}
            Name: {secondFixture.Name}
            Address: {secondFixture.Address}
            VAT number: {secondFixture.VatNumber}
            Email: {secondFixture.Email}{Environment.NewLine}
            """, capturedStdOut);
    }

    [Test]
    public void ModifiesClient()
    {
        dynamic fixture = fixtures["clients"]["one"];
        var newName = "new name";
        var newAddress = "new address";
        var newVatNumber = "new vat";
        Assert.AreNotEqual(newName, fixture.Name);
        Assert.AreNotEqual(newAddress, fixture.Address);
        Assert.AreNotEqual(newVatNumber, fixture.VatNumber);

        var stdIn = $"""
            {newName}
            {newAddress}
            {newVatNumber}
            """;

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "client", "modify", fixture.Id.ToString() });
            });
        });

        Assert.AreEqual($"""
            Enter new client name:
            Enter new client address:
            Enter new client VAT number:
            Client {newName} has been modified.
            """, capturedStdOut);
        fixture = ClientFixture(fixture.Id);
        Assert.AreEqual(newName, fixture.Name);
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void DeletesUninvoicedSupplier()
    {
        CreateSupplierFixtures();
        dynamic fixture = fixtures["suppliers"]["one"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "supplier", "delete", fixture.Id.ToString() });
        });

        Assert.AreEqual($"Supplier {fixture.Name} has been deleted.", capturedStdOut);
        Assert.AreEqual(fixtures["suppliers"].Count - 1, (long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar(), "Supplier should have been deleted.");
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void DeletesUninvoicedClient()
    {
        CreateClientFixtures();
        dynamic client = fixtures["clients"]["one"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "client", "delete", client.Id.ToString() });
        });

        Assert.AreEqual($"Client {client.Name} has been deleted.", capturedStdOut);
        Assert.AreEqual(fixtures["clients"].Count - 1, (long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM clients").ExecuteScalar(), "Client should have been deleted.");
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void RequiresVatRateEnv()
    {
        Environment.SetEnvironmentVariable("STANDARD_VAT_RATE", null);

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp();
        });

        Assert.AreEqual($"STANDARD_VAT_RATE env var must be set{Environment.NewLine}", capturedStdOut);
    }

    [Test, Ignore("It's not clear how to fully isolate original db/migrations path in integration tests")]
    public void InitializesMigrations()
    {
        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "migration", "init" });
        });

        Assert.AreEqual("Migrations have been initialized.", capturedStdOut);
    }

    [Test, Ignore("See above")]
    public void CreatesEmptyMigration()
    {
        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "migration", "create", "test name" });
        });

        Assert.AreEqual("Migration has been created.", capturedStdOut);
    }

    [Test, Ignore("See above")]
    public void NoPendingMigrations()
    {
        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "migration", "apply" });
        });

        Assert.AreEqual("There are no pending migrations.", capturedStdOut);
    }

    [Test]
    public void MarksInvoicePaid()
    {
        dynamic fixture = fixtures["invoices"]["one"];
        Assert.False(fixture.Paid);

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "paid", fixture.Id.ToString() });
        });

        fixture = InvoiceFixture(fixture.Id);
        Assert.True(fixture.Paid, "Invoice should be paid");
        Assert.NotNull(fixture.PaidDate);
        Assert.AreEqual($"Invoice {fixture.Number} has been marked as paid.{Environment.NewLine}", capturedStdOut);
    }

    [Test]
    public void InvoicePaidCommandNoId()
    {
        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "paid" });
        });

        Assert.AreEqual($"Please provide invoice id.{Environment.NewLine}", capturedStdOut);
    }

    [Test]
    public void InvoicePaidCommandInvalidId()
    {
        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "paid", "invalid_id" });
        });

        Assert.AreEqual($"Invalid invoice id.{Environment.NewLine}", capturedStdOut);
    }

    [Test]
    public void MigrationCreateCommandNoName()
    {
        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "migration", "create" });
        });

        Assert.AreEqual($"Please provide migration name.{Environment.NewLine}", capturedStdOut);
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void ValidatesUserInputOnSupplierCreation()
    {
        var stdIn = " ";

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "supplier", "create" });
            });
        });

        Assert.AreEqual($"""
            Enter supplier name:
            Value cannot be empty.{Environment.NewLine}
            """, capturedStdOut);
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void ValidatesUserInputOnClientCreation()
    {
        var stdOut = " ";

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdOut, () =>
            {
                RunApp(arguments: new string[] { "client", "create" });
            });
        });

        Assert.AreEqual($"""
            Enter client name:
            Value cannot be empty.{Environment.NewLine}
            """, capturedStdOut);
    }

    [Test]
    public void SendsEmailWithInvoice()
    {
        dynamic fixture = fixtures["invoices"]["one"];
        // Port is specified in `.runsettings`.
        using var fakeSmtpServer = SimpleSmtpServer.Start(port: 1024);

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "send", fixture.Id.ToString() });
        });

        Assert.AreEqual(1, fakeSmtpServer.ReceivedEmailCount, "Should send an email");
        Assert.AreEqual($"Invoice has been sent to the client.{Environment.NewLine}", capturedStdOut);
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void ClientCreateCommandInvalidEmail()
    {
        var stdIn = $"""
            {ValidName()}
            {ValidAddress()}
            {ValidVatNumber()}
            invalid email
            """;

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "client", "create" });
            });
        });

        StringAssert.Contains("Email has invalid format.", capturedStdOut);
    }

    [Test]
    [Property("SkipFixtureCreation", "true")]
    public void SupplierCreateCommandInvalidVatNumber()
    {
        var stdIn = $"""
            {ValidName()}
            {ValidAddress()}
            invalid_vat_number
            {ValidIban()}
            """;

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "supplier", "create" });
            });
        });

        StringAssert.Contains("VAT number has invalid format.", capturedStdOut);
    }

    // [SetUp]
    // protected void SetUp()
    // {
    //     migrationsPath = Path.Combine(Path.GetTempPath(), new Random().Next().ToString());
    //     Directory.CreateDirectory(migrationsPath);
    //     Environment.CurrentDirectory = migrationsPath;
    // }

    // [TearDown]
    // protected void DeleteMigrationsRootDirectory()
    // {
    //     new DirectoryInfo(migrationsPath).Delete(recursive: true);
    // }

    IImmutableSet<string> SupportedCommands()
    {
        return ImmutableHashSet.Create("supplier create", "client create", "invoice create",
            "invoice pdf", "invoice details", "invoice list", "supplier modify", "supplier list", "client list", "client modify", "supplier delete", "client delete", "migration init", "migration create", "migration apply",
            "invoice paid", "invoice send");
    }

    string ListDelimiter()
    {
        return new string('-', 50);
    }
}
