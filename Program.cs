using System.Collections.Immutable;
using System.Globalization;
using Intech.Invoice;
using Intech.Invoice.DbMigration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Npgsql;

var dbConnectionString = new DbConnString(host: Environment.GetEnvironmentVariable("PG_HOST"),
    user: Environment.GetEnvironmentVariable("PG_USER"),
    password: Environment.GetEnvironmentVariable("PG_PASSWORD"),
    db: Environment.GetEnvironmentVariable("PG_DATABASE"));
using var pgDataSource = NpgsqlDataSource.Create(dbConnectionString.Npgsql());

var envCulture = Environment.GetEnvironmentVariable("CULTURE");

if (envCulture is not null)
{
    CultureInfo.CurrentCulture = new CultureInfo(envCulture);
}

var timezone = Timezone.Default();
var systemClock = new SystemClock(timezone);

var supportedCommands = ImmutableHashSet.Create("supplier create", "client create", "invoice create",
    "invoice pdf", "invoice details", "invoice list", "supplier modify", "supplier list", "client list", "client modify", "supplier delete", "client delete", "migration init", "migration create", "migration apply",
    "invoice paid", "invoice send");
var currentCommand = string.Join(" ", args.Take(2));

var migrations = new Migrations(Path.Combine(Environment.CurrentDirectory, "db", "migrations"), pgDataSource);
var pgDump = new PgDump(dbConnectionString.PgDump());
var pgSchema = new PgSchema(Path.Combine("db", "schema.sql"), pgDataSource, pgDump);

try
{
    if (Environment.GetEnvironmentVariable("STANDARD_VAT_RATE") is null)
    {
        throw new Exception("STANDARD_VAT_RATE env var must be set");
    }

    var standardVatRate = int.Parse(Environment.GetEnvironmentVariable("STANDARD_VAT_RATE"));


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
                    var supplierName = new Nonblank(new ConsoleInput(Console.ReadLine())).ToString();

                    Console.WriteLine("Enter supplier address:");
                    var supplierAddress = new Nonblank(new ConsoleInput(Console.ReadLine())).ToString();

                    Console.WriteLine("Enter supplier VAT number:");
                    var supplierVatNumber = new StrictInputVatNumber(new Nonblank(new ConsoleInput(Console.ReadLine()))).ToString();

                    Console.WriteLine("Enter supplier IBAN:");
                    var supplierIban = new StrictInputIban(new Nonblank(new ConsoleInput(Console.ReadLine()))).ToString();

                    Console.WriteLine("Enter supplier email:");
                    var supplierEmail = new StrictInputEmail(new Nonblank(new ConsoleInput(Console.ReadLine()))).ToString();

                    var supplier = new UniqPgSuppliers(new PgSuppliers(pgDataSource), pgDataSource).Add(supplierName, supplierAddress, supplierVatNumber, supplierIban, supplierEmail);

                    Console.Write($"Supplier {supplier} has been created.");
                    break;
                }
            case "client create":
                {
                    Console.WriteLine("Enter client name:");
                    var clientName = new Nonblank(new ConsoleInput(Console.ReadLine())).ToString();

                    Console.WriteLine("Enter client address:");
                    var clientAddress = new Nonblank(new ConsoleInput(Console.ReadLine())).ToString();

                    Console.WriteLine("Enter client VAT number:");
                    var clientVatNumber = new Nonblank(new ConsoleInput(Console.ReadLine())).ToString();

                    Console.WriteLine("Enter client email:");
                    var email = new StrictInputEmail(new Nonblank(new ConsoleInput(Console.ReadLine()))).ToString();

                    var pgClients = new UniqPgClients(new PgClients(pgDataSource), pgDataSource);
                    var client = pgClients.Add(clientName, clientAddress, clientVatNumber, email);

                    Console.Write($"Client {client} has been created.");
                    break;
                }
            case "invoice create":
                {
                    Console.WriteLine($"Enter supplier id{new ListHint<Supplier>(new PgSuppliers(pgDataSource))}:");
                    int supplierId = int.Parse(Console.ReadLine());

                    Console.WriteLine($"Enter client id{new ListHint<Client>(new PgClients(pgDataSource))}:");
                    int clientId = int.Parse(Console.ReadLine());

                    Console.WriteLine($"Enter VAT rate as positive integer, \"reverse-charged\" string or leave blank to apply the standard rate of {standardVatRate}%:");
                    var vatRateInput = new VatRateInput(Console.ReadLine());
                    var vatRate = vatRateInput.VatRate();

                    Console.WriteLine("Enter line item name:");
                    string lineItemName = new Nonblank(new ConsoleInput(Console.ReadLine())).ToString();

                    Console.WriteLine("Enter line item price (integer):");
                    int lineItemPrice = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter line item quantity (integer):");
                    int lineItemQuantity = int.Parse(Console.ReadLine());

                    var invoiceDate = systemClock.TodayInAppTimeZone();

                    new PgTransaction(pgDataSource).Wrap(() =>
                    {
                        var invoice = new PgInvoices(pgDataSource).Add(
                               number: new TimestampedNumber(systemClock).ToString(),
                               date: invoiceDate,
                               dueDate: DueDate.Standard(invoiceDate).Date(),
                               vatRate: vatRate.IntValue(),
                               supplierId: supplierId,
                               clientId: clientId);
                        new PgLineItems(pgDataSource).Add(invoice.Id(), lineItemName, lineItemPrice, lineItemQuantity);

                        Console.Write($"Invoice {invoice} has been issued.");
                    });

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

                    var pgInvoice = new PgInvoice(invoiceId, pgDataSource);
                    Console.Write(pgInvoice.Print(new ConsoleMedia()).Text());

                    break;
                }
            case "invoice list":
                {
                    new ConsoleDelimitedList<Invoice>(new PgInvoices(pgDataSource)).Print();
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

                    Console.WriteLine("Enter new supplier email:");
                    var newEmail = Console.ReadLine();

                    var supplier = new PgSupplier(id, pgDataSource);
                    supplier.Modify(newName, newAddress, newVatNumber, newIban, newEmail);

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
            case "client modify":
                {
                    int id = int.Parse(args[2]);

                    Console.WriteLine("Enter new client name:");
                    var newName = Console.ReadLine();

                    Console.WriteLine("Enter new client address:");
                    var newAddress = Console.ReadLine();

                    Console.WriteLine("Enter new client VAT number:");
                    var newVatNumber = Console.ReadLine();

                    var client = new PgClient(id, pgDataSource);
                    client.Modify(newName, newAddress, newVatNumber);

                    Console.Write($"Client {client} has been modified.");

                    break;
                }
            case "supplier delete":
                {
                    int id = int.Parse(args[2]);

                    var supplier = new PgSupplier(id, pgDataSource);
                    var name = supplier.Name();
                    supplier.Delete();

                    Console.Write($"Supplier {name} has been deleted.");

                    break;
                }
            case "client delete":
                {
                    int id = int.Parse(args[2]);

                    var client = new PgClient(id, pgDataSource);
                    var name = client.Name();
                    client.Delete();

                    Console.Write($"Client {name} has been deleted.");

                    break;
                }
            case "migration init":
                {
                    new FileMigration(Path.Combine("assets", "initial_migration.pgsql"), pgDataSource).Apply();
                    migrations.Init();
                    pgSchema.Generate();

                    Console.WriteLine("Migrations have been initialized.");

                    break;
                }
            case "migration create":
                {
                    var invalidArgs = args.Length < 3;

                    if (invalidArgs)
                    {
                        throw new Exception("Please provide migration name.");
                    }

                    var name = args[2];

                    migrations.CreateEmpty(new TimestampedId(name, systemClock));
                    Console.WriteLine("Migration has been created.");

                    break;
                }
            case "migration apply":
                {
                    new Pending(migrations).Apply(whenAny: (applied) =>
                    {
                        foreach (var migration in applied)
                        {
                            Console.WriteLine($"Migration {migration} has been applied.");
                        }

                        pgSchema.Regenerate();
                    },
                    whenNone: () => Console.WriteLine("There are no pending migrations."));

                    break;
                }
            case "invoice paid":
                {
                    var invalidArgs = args.Length < 3;

                    if (invalidArgs)
                    {
                        throw new Exception("Please provide invoice id.");
                    }

                    int id;

                    try
                    {
                        id = int.Parse(args[2]);
                    }
                    catch (FormatException e)
                    {
                        throw new Exception("Invalid invoice id.", e);
                    }

                    var pgInvoice = new PgInvoice(id, pgDataSource);
                    pgInvoice.MarkPaid(systemClock.TodayInAppTimeZone());

                    Console.WriteLine($"Invoice {pgInvoice} has been marked as paid.");

                    break;
                }
            case "invoice send":
                {
                    var id = int.Parse(args[2]);

                    var host = Environment.GetEnvironmentVariable("SMTP_HOST");
                    var port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));
                    var username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
                    var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
                    var secure = bool.Parse(Environment.GetEnvironmentVariable("SMTP_SECURE"));
                    using var smtpClient = new SmtpClient();
                    // TODO Get rid of the conditions.
                    smtpClient.Connect(host: host, port: port, secure ? SecureSocketOptions.Auto : SecureSocketOptions.None);
                    if (secure) smtpClient.Authenticate(userName: username, password: password);

                    var pgInvoice = new PgInvoice(id, pgDataSource);
                    pgInvoice.Send(smtpClient);

                    smtpClient.Disconnect(quit: true);

                    Console.WriteLine($"Invoice has been sent to the client.");

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
        Console.WriteLine(e);
    }
    else
    {
        Console.WriteLine(e.Message);
    }

    // Console.Error.WriteLine(e.Message);
}
