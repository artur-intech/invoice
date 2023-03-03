using System.Collections.Immutable;
using System.Globalization;
using Intech.Invoice;
using Npgsql;

var envPgHost = Environment.GetEnvironmentVariable("PG_HOST");
var envPgUser = Environment.GetEnvironmentVariable("PG_USER");
var envPgPassword = Environment.GetEnvironmentVariable("PG_PASSWORD");
var envPgDatabase = Environment.GetEnvironmentVariable("PG_DATABASE");
var dbConnectionString = $"Server={envPgHost}; User Id={envPgUser}; Password={envPgPassword}; Database={envPgDatabase}";
using var pgDataSource = NpgsqlDataSource.Create(dbConnectionString);

var envCulture = Environment.GetEnvironmentVariable("CULTURE");

if (envCulture is not null)
{
    CultureInfo.CurrentCulture = new CultureInfo(envCulture);
}

var envTimeZone = Environment.GetEnvironmentVariable("TIME_ZONE");
var timeZone = envTimeZone is not null ? TimeZoneInfo.FindSystemTimeZoneById(envTimeZone) : TimeZoneInfo.Local;

var systemClock = new SystemClock(timeZone);

var supportedCommands = ImmutableHashSet.Create("supplier create", "client create", "invoice create",
    "invoice pdf", "invoice details", "invoice list", "supplier modify", "supplier list", "client list");
var currentCommand = string.Join(" ", args.Take(2));

try
{
    if (args.Length < 2 || !supportedCommands.Contains(currentCommand))
    {
        Console.Write($"""
            Please provide one of the supported commands:
            {string.Join("\n", supportedCommands)}.
            """);
    }
    else
    {
        switch (currentCommand)
        {
            case "supplier create":
                {
                    Console.WriteLine("Enter supplier name:");
                    var supplierName = Console.ReadLine();

                    Console.WriteLine("Enter supplier address:");
                    var supplierAddress = Console.ReadLine();

                    Console.WriteLine("Enter supplier VAT number:");
                    var supplierVatNumber = Console.ReadLine();

                    Console.WriteLine("Enter supplier IBAN:");
                    var supplierIban = Console.ReadLine();

                    var supplier = new StrictPgSuppliers(new UniqPgSuppliers(new PgSuppliers(pgDataSource), pgDataSource)).Add(supplierName, supplierAddress, supplierVatNumber, supplierIban);

                    Console.Write($"Supplier {supplier} has been created.");
                    break;
                }
            case "client create":
                {
                    Console.WriteLine("Enter client name:");
                    var clientName = Console.ReadLine();

                    Console.WriteLine("Enter client address:");
                    var clientAddress = Console.ReadLine();

                    Console.WriteLine("Enter client VAT number:");
                    var clientVatNumber = Console.ReadLine();

                    var pgClients = new StrictPgClients(new UniqPgClients(new PgClients(pgDataSource), pgDataSource));
                    var client = pgClients.Add(clientName, clientAddress, clientVatNumber);

                    Console.Write($"Client {client} has been created.");
                    break;
                }
            case "invoice create":
                {
                    Console.WriteLine($"Enter supplier id{new SupplierHint(new PgSuppliers(pgDataSource))}:");
                    int supplierId = int.Parse(Console.ReadLine());

                    Console.WriteLine($"Enter client id{new ClientHint(new PgClients(pgDataSource))}:");
                    int clientId = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter line item name:");
                    string lineItemName = Console.ReadLine();

                    Console.WriteLine("Enter line item price:");
                    int lineItemPrice = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter line item quantity:");
                    int lineItemQuantity = int.Parse(Console.ReadLine());

                    var invoiceDate = systemClock.TodayInAppTimeZone();
                    using var pgConnection = pgDataSource.OpenConnection();

                    using var pgTransaction = pgConnection.BeginTransaction();
                    var invoice = new PgInvoices(pgDataSource).Add(
                        number: new TimestampedNumber(systemClock).ToString(),
                        date: invoiceDate,
                        dueDate: DueDate.Standard(invoiceDate).Date(),
                        vatRate: new ReverseChargedVatRate().IntValue(),
                        supplierId: supplierId,
                        clientId: clientId);
                    new PgLineItems(pgDataSource).Add(invoice.Id(), lineItemName, lineItemPrice, lineItemQuantity);
                    pgTransaction.Commit();

                    Console.Write($"Invoice {invoice} has been issued.");
                    break;
                }
            case "invoice pdf":
                {
                    int invoiceId = int.Parse(args[2]);
                    var invoice = new PgInvoice(invoiceId, pgDataSource);
                    invoice.SavePdf();

                    Console.Write($"Invoice PDF has been saved.");
                    break;
                }
            case "invoice details":
                {
                    int invoiceId = int.Parse(args[2]);
                    new ConsoleInvoiceDetails(new PgInvoice(invoiceId, pgDataSource), new ConsoleMedia()).Print();
                    break;
                }
            case "invoice list":
                {
                    new ConsoleDelimitedList<Invoice>(new PgInvoices(pgDataSource).Fetch()).Print();
                    break;
                }
            case "supplier modify":
                {
                    int id = int.Parse(args[2]);

                    Console.WriteLine("Enter new supplier name:");
                    var newName = Console.ReadLine();

                    Console.WriteLine("Enter new supplier address:");
                    var newAddress = Console.ReadLine();

                    Console.WriteLine("Enter new supplier VAT number:");
                    var newVatNumber = Console.ReadLine();

                    Console.WriteLine("Enter new supplier IBAN:");
                    var newIban = Console.ReadLine();

                    var supplier = new PgSupplier(id, pgDataSource);
                    supplier.Modify(newName, newAddress, newVatNumber, newIban);

                    Console.Write($"Supplier {supplier} has been modified.");

                    break;
                }
            case "supplier list":
                {
                    new ConsoleDelimitedList<Supplier>(new PgSuppliers(pgDataSource)).Print();
                    break;
                }
            case "client list":
                {
                    new ConsoleDelimitedList<Client>(new PgClients(pgDataSource)).Print();
                    break;
                }
        }
    }
}
catch (Exception e)
{
    var showDetailedExceptions = Convert.ToBoolean(Environment.GetEnvironmentVariable("SHOW_DETAILED_EXCEPTIONS")) || false;

    if (showDetailedExceptions)
    {
        Console.Write(e);
    }
    else
    {
        Console.Write(e.Message);
    }
}