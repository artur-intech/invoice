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

        var name = "new supplier";
        var stdIn = $"""
            {name}
            Main street 1
            US1234
            US12345
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

        var name = "new client";
        var stdIn = $"""
            {name}
            address
            vat_number
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
        dynamic client = fixtures["clients"]["one"];
        var stdIn = $"""
            {firstSupplier.Id}
            {client.Id}
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
            Enter client id:
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
        dynamic invoice = fixtures["invoices"]["one"];

        var capturedStdOut = CapturedStdOut(() =>
        {
            RunApp(arguments: new string[] { "invoice", "list" });
        });

        Assert.AreEqual($"""
            Client: {client.Name}
            Number: {invoice.Number}
            Date: {invoice.Date}
            Due date: {invoice.DueDate}
            Total: {invoice.Total}
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

    IImmutableSet<string> SupportedCommands()
    {
        return ImmutableHashSet.Create("supplier create", "client create", "invoice create",
            "invoice pdf", "invoice details", "invoice list");
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
}