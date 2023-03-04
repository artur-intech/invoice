using System.Collections.Immutable;
using System.Dynamic;
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

        dynamic createdDbRow = LastInvoiceDbRow();
        Assert.AreEqual($"""
            Enter supplier id ({firstSupplier.Id} - "{firstSupplier.Name}", {secondSupplier.Id} - "{secondSupplier.Name}"):
            Enter client id ({firstClient.Id} - "{firstClient.Name}", {secondClient.Id} - "{secondClient.Name}"):
            Enter line item name:
            Enter line item price:
            Enter line item quantity:
            Invoice {createdDbRow.Number} has been issued.
            """, capturedStdOut);
        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM invoices").ExecuteScalar());
        Assert.AreEqual(1, pgDataSource.CreateCommand("SELECT COUNT(*) FROM line_items").ExecuteScalar());
    }

    [Test]
    public void SavesInvoicePdf()
    {
        dynamic invoice = fixtures["invoices"]["one"];
        var invoicePdfFilePath = $"{invoice.SupplierName}_invoice_{invoice.Number}.pdf";
        FileAssert.DoesNotExist(invoicePdfFilePath);

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "pdf", invoice.Id.ToString() });
        });

        Assert.AreEqual("Invoice PDF has been saved.", capturedStdOut);
        FileAssert.Exists(invoicePdfFilePath);

        File.Delete(invoicePdfFilePath);
    }

    [Test]
    public void PrintsInvoiceDetails()
    {
        dynamic client = fixtures["clients"]["one"];
        dynamic invoice = fixtures["invoices"]["one"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "details", invoice.Id.ToString() });
        });

        Assert.AreEqual($"""
            Id: {invoice.Id}
            Client: {client.Name}
            Number: {invoice.Number}
            Date: {invoice.Date}
            Due date: {invoice.DueDate}
            Total: {invoice.Total}
            """, capturedStdOut);
    }
    [Test]
    public void PrintsAllInvoices()
    {
        dynamic client = fixtures["clients"]["one"];
        dynamic firstInvoice = fixtures["invoices"]["one"];
        dynamic secondInvoice = fixtures["invoices"]["two"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "list" });
        });

        Assert.AreEqual($"""
            Id: {firstInvoice.Id}
            Client: {client.Name}
            Number: {firstInvoice.Number}
            Date: {firstInvoice.Date}
            Due date: {firstInvoice.DueDate}
            Total: {firstInvoice.Total}
            {ListDelimiter()}
            Id: {secondInvoice.Id}
            Client: {client.Name}
            Number: {secondInvoice.Number}
            Date: {secondInvoice.Date}
            Due date: {secondInvoice.DueDate}
            Total: {secondInvoice.Total}
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
    [Property("SkipFixtureCreation", "true")]
    public void ValidatesUserInput()
    {
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar());

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
            Enter supplier address:
            Enter supplier VAT number:
            Enter supplier IBAN:
            Supplier name cannot be empty.
            """, capturedStdOut);
        Assert.Zero((long)pgDataSource.CreateCommand("SELECT COUNT(*) FROM suppliers").ExecuteScalar());
    }

    [Test]
    public void ModifiesSupplier()
    {
        dynamic supplier = fixtures["suppliers"]["one"];
        var newName = "new name";
        var newAddress = "new address";
        var newVatNumber = "new vat";
        var newIban = "new iban";
        Assert.AreNotEqual(newName, supplier.Name);
        Assert.AreNotEqual(newAddress, supplier.Address);
        Assert.AreNotEqual(newVatNumber, supplier.VatNumber);
        Assert.AreNotEqual(newIban, supplier.Iban);

        var stdIn = $"""
            {newName}
            {newAddress}
            {newVatNumber}
            {newIban}
            """;

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "supplier", "modify", supplier.Id.ToString() });
            });
        });

        Assert.AreEqual($"""
            Enter new supplier name:
            Enter new supplier address:
            Enter new supplier VAT number:
            Enter new supplier IBAN:
            Supplier {newName} has been modified.
            """, capturedStdOut);
        supplier = new SupplierFixtures(pgDataSource).Fetch(supplier.Id);
        Assert.AreEqual(newName, supplier.Name);
    }

    [Test]
    public void PrintsAllSuppliers()
    {
        dynamic firstSupplier = fixtures["suppliers"]["one"];
        dynamic secondSupplier = fixtures["suppliers"]["two"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "supplier", "list" });
        });

        Assert.AreEqual($"""
            Id: {firstSupplier.Id}
            Name: {firstSupplier.Name}
            Address: {firstSupplier.Address}
            VAT number: {firstSupplier.VatNumber}
            IBAN: {firstSupplier.Iban}
            {ListDelimiter()}
            Id: {secondSupplier.Id}
            Name: {secondSupplier.Name}
            Address: {secondSupplier.Address}
            VAT number: {secondSupplier.VatNumber}
            IBAN: {secondSupplier.Iban}
            """, capturedStdOut);
    }

    [Test]
    public void PrintsAllClients()
    {
        dynamic firstClient = fixtures["clients"]["one"];
        dynamic secondClient = fixtures["clients"]["two"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "client", "list" });
        });

        Assert.AreEqual($"""
            Id: {firstClient.Id}
            Name: {firstClient.Name}
            Address: {firstClient.Address}
            VAT number: {firstClient.VatNumber}
            {ListDelimiter()}
            Id: {secondClient.Id}
            Name: {secondClient.Name}
            Address: {secondClient.Address}
            VAT number: {secondClient.VatNumber}
            """, capturedStdOut);
    }

    [Test]
    public void ModifiesClient()
    {
        dynamic client = fixtures["clients"]["one"];
        var newName = "new name";
        var newAddress = "new address";
        var newVatNumber = "new vat";
        Assert.AreNotEqual(newName, client.Name);
        Assert.AreNotEqual(newAddress, client.Address);
        Assert.AreNotEqual(newVatNumber, client.VatNumber);

        var stdIn = $"""
            {newName}
            {newAddress}
            {newVatNumber}
            """;

        var capturedStdOut = CapturedStdOut(() =>
        {
            SubstituteStdIn(stdIn, () =>
            {
                RunApp(arguments: new string[] { "client", "modify", client.Id.ToString() });
            });
        });

        Assert.AreEqual($"""
            Enter new client name:
            Enter new client address:
            Enter new client VAT number:
            Client {newName} has been modified.
            """, capturedStdOut);
        client = new ClientFixtures(pgDataSource).Fetch(client.Id);
        Assert.AreEqual(newName, client.Name);
    }

    [Test]
    public void DeletesSupplier()
    {
        dynamic supplier = fixtures["suppliers"]["one"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "supplier", "delete", supplier.Id.ToString() });
        });

        Assert.AreEqual($"Supplier {supplier.Name} has been deleted.", capturedStdOut);
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

    IImmutableSet<string> SupportedCommands()
    {
        return ImmutableHashSet.Create("supplier create", "client create", "invoice create",
            "invoice pdf", "invoice details", "invoice list", "supplier modify", "supplier list", "client list", "client modify", "supplier delete", "client delete");
    }

    ExpandoObject LastInvoiceDbRow()
    {
        using var command = pgDataSource.CreateCommand("SELECT * FROM invoices ORDER BY id DESC LIMIT 1");
        using var reader = command.ExecuteReader();
        reader.Read();

        dynamic invoice = new ExpandoObject();
        invoice.Number = reader["number"];

        return invoice;
    }

    string ListDelimiter()
    {
        return new string('-', 50);
    }
}