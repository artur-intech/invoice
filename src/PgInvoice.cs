using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Svg.Converter;
using MailKit.Net.Smtp;
using MimeKit;
using Npgsql;

namespace Intech.Invoice;

sealed class PgInvoice : Invoice
{
    readonly int id;
    readonly NpgsqlDataSource pgDataSource;

    public PgInvoice(int id, NpgsqlDataSource pgDataSource)
    {
        this.id = id;
        this.pgDataSource = pgDataSource;
    }

    public int Id()
    {
        return id;
    }

    public override string ToString()
    {
        return Number();
    }

    string Number()
    {
        using var command = pgDataSource.CreateCommand("SELECT number FROM invoices WHERE id = $1");
        command.Parameters.AddWithValue(id);
        return (string)command.ExecuteScalar();
    }

    public void SavePdf()
    {
        var sql = """
            SELECT
                number,
                supplier_name
            FROM
                invoices
            WHERE
                invoices.id = $1
            """;

        using var cmd = pgDataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue(id);
        using var reader = cmd.ExecuteReader();
        reader.Read();
        var number = (string)reader["number"];
        var supplierName = (string)reader["supplier_name"];

        var filename = $"{supplierName}_invoice_{number}.pdf";


        File.WriteAllBytes(filename, Pdf().ToArray());
    }

    MemoryStream Pdf()
    {
        var sql = """
            SELECT
            invoices.*,
            SUM(price * quantity::int) AS subtotal,
            (SUM(price * quantity::int) * vat_rate) / 100 AS vat_amount,
            SUM(price * quantity::int) + ((SUM(price * quantity::int) * vat_rate) / 100) AS total
            FROM
            invoices
            LEFT JOIN
            line_items ON invoices.id = line_items.invoice_id
            WHERE
            invoices.id = $1
            GROUP BY
            invoices.id
            """;

        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();
        reader.Read();
        var number = (string)reader["number"];
        var date = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date"));
        var dueDate = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("due_date"));
        var vatRate = (short)reader["vat_rate"];
        var subtotal = (long)reader["subtotal"];
        var vatAmount = (long)reader["vat_amount"];
        var total = (long)reader["total"];
        var supplierName = (string)reader["supplier_name"];
        var supplierAddress = (string)reader["supplier_address"];
        var supplierVatNumber = (string)reader["supplier_vat_number"];
        var supplierIban = (string)reader["supplier_iban"];
        var clientName = (string)reader["client_name"];
        var clientAddress = (string)reader["client_address"];
        var clientVatNumber = (string)reader["client_vat_number"];

        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf);
        var helveticaBold = PdfFontFactory.CreateRegisteredFont("helvetica-bold");
        var dimgray = WebColors.GetRGBColor("dimgray");

        using var intechLogoFile = File.Open(Path.Join("assets", "intech_logo.svg"), FileMode.Open);
        var intechLogo = SvgConverter.ConvertToImage(intechLogoFile, pdf);

        var details = new Table(2);
        details.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
        details.AddCell(new Cell().Add(new Paragraph("Number:").SetPaddingRight(15)
        .SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
        details.AddCell(new Cell().Add(new Paragraph(number)).SetBorder(Border.NO_BORDER));
        details.AddCell(new Cell().Add(new Paragraph("Date:").SetPaddingRight(15)
        .SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
        details.AddCell(new Cell().Add(new Paragraph(date.ToString())).SetBorder(Border.NO_BORDER));
        details.AddCell(new Cell().Add(new Paragraph("Due date:").SetPaddingRight(15)
        .SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
        details.AddCell(new Cell().Add(new Paragraph(dueDate.ToString())).SetBorder(Border.NO_BORDER));

        var supplier = new Cell();
        supplier.SetBorder(Border.NO_BORDER);
        supplier.Add(new Paragraph(supplierName));
        supplier.Add(new Paragraph(supplierAddress));
        supplier.Add(new Paragraph(supplierVatNumber));
        supplier.Add(new Paragraph($"IBAN: {supplierIban}"));

        var client = new Cell();
        client.SetBorder(Border.NO_BORDER);
        client.Add(new Paragraph("Client:").SetMarginTop(4).SetMarginBottom(2)
        .SetFontColor(WebColors.GetRGBColor("darkgray")));
        client.Add(new Paragraph(clientName));
        client.Add(new Paragraph(clientAddress));
        client.Add(new Paragraph(clientVatNumber));

        var detailsContainer = new Table(2);
        detailsContainer.UseAllAvailableWidth();
        detailsContainer.AddCell(new Cell().Add(intechLogo).SetBorder(Border.NO_BORDER));
        detailsContainer.AddCell(new Cell().Add(new Paragraph("Invoice").SetFontSize(24)
        .SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
        detailsContainer.AddCell(supplier);
        detailsContainer.AddCell(new Cell().Add(details).SetBorder(Border.NO_BORDER));
        detailsContainer.AddCell(client);
        document.Add(detailsContainer);

        var lineItems = new Table(UnitValue.CreatePercentArray(new float[] { 55, 15, 15, 15 }));
        lineItems.UseAllAvailableWidth();
        lineItems.SetMarginTop(15);

        lineItems.AddCell(new Cell().Add(new Paragraph("Service description").SetFontSize(10))
        .SetBackgroundColor(dimgray)
        .SetFontColor(ColorConstants.WHITE)
        .SetBorderBottomLeftRadius(new BorderRadius(4f))
        .SetBorderTopLeftRadius(new BorderRadius(4f))
        .SetBorder(Border.NO_BORDER)
        .SetPadding(5));
        lineItems.AddCell(new Cell().Add(new Paragraph("Quantity").SetFontSize(10))
        .SetBackgroundColor(dimgray)
        .SetFontColor(ColorConstants.WHITE)
        .SetBorder(Border.NO_BORDER)
        .SetPadding(5));
        lineItems.AddCell(new Cell().Add(new Paragraph("Rate").SetFontSize(10))
        .SetBackgroundColor(dimgray)
        .SetFontColor(ColorConstants.WHITE)
        .SetBorder(Border.NO_BORDER)
        .SetPadding(5));
        lineItems.AddCell(new Cell().Add(new Paragraph("Amount").SetFontSize(10))
        .SetBackgroundColor(dimgray)
        .SetFontColor(ColorConstants.WHITE)
        .SetBorderBottomRightRadius(new BorderRadius(4f))
        .SetBorderTopRightRadius(new BorderRadius(4f))
        .SetBorder(Border.NO_BORDER)
        .SetPadding(5));

        using var commandLineItems = pgDataSource.CreateCommand("SELECT * FROM line_items WHERE invoice_id = $1");
        commandLineItems.Parameters.AddWithValue(id);
        using var readerLineItems = commandLineItems.ExecuteReader();

        while (readerLineItems.Read())
        {
            var name = readerLineItems.GetString(readerLineItems.GetOrdinal("name"));
            var price = readerLineItems.GetInt32(readerLineItems.GetOrdinal("price"));
            var quantity = readerLineItems.GetInt32(readerLineItems.GetOrdinal("quantity"));
            var amount = price * quantity;

            lineItems.AddCell(new Cell().Add(new Paragraph(name)).SetBorder(Border.NO_BORDER).SetPadding(5));
            lineItems.AddCell(new Cell().Add(new Paragraph($"{quantity} h")).SetBorder(Border.NO_BORDER).SetPadding(5));
            lineItems.AddCell(new Cell().Add(new Paragraph($"{price} €")).SetBorder(Border.NO_BORDER).SetPadding(5));
            lineItems.AddCell(new Cell().Add(new Paragraph($"{amount} €")).SetBorder(Border.NO_BORDER).SetPadding(5));
        }

        lineItems.AddCell(new Cell(1, 4).SetHeight(15).SetBorder(Border.NO_BORDER));

        lineItems.AddCell(new Cell(1, 3).Add(new Paragraph("Subtotal:")
        .SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER).SetPaddingRight(15));
        lineItems.AddCell(new Cell().Add(new Paragraph($"{subtotal} €")).SetBorder(Border.NO_BORDER));

        lineItems.AddCell(new Cell(1, 3).Add(new Paragraph($"VAT ({vatRate} %):")
        .SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER).SetPaddingRight(15));
        lineItems.AddCell(new Cell().Add(new Paragraph($"{vatAmount} €")).SetBorder(Border.NO_BORDER));

        lineItems.AddCell(new Cell(1, 3).Add(new Paragraph("Total:").SetFont(helveticaBold)
        .SetFontSize(14)
        .SetTextAlignment(TextAlignment.RIGHT))
        .SetBorder(Border.NO_BORDER).SetPaddingRight(15));
        lineItems.AddCell(new Cell().Add(new Paragraph($"{total} €").SetFont(helveticaBold)
        .SetFontSize(14))
        .SetBorder(Border.NO_BORDER));

        document.Add(lineItems);

        document.Add(new Paragraph("Reverse charge"));

        return stream;
    }

    public ConsoleMedia Print(ConsoleMedia media)
    {
        var sql = """
            SELECT
            invoices.*,
            SUM(price * quantity::int) AS subtotal,
            (SUM(price * quantity::int) * vat_rate) / 100 AS vat_amount,
            SUM(price * quantity::int) + ((SUM(price * quantity::int) * vat_rate) / 100) AS total
            FROM
            invoices
            LEFT JOIN
            line_items ON invoices.id = line_items.invoice_id
            WHERE
            invoices.id = $1
            GROUP BY
            invoices.id
            """;

        using var command = pgDataSource.CreateCommand(sql);
        command.Parameters.AddWithValue(id);
        using var reader = command.ExecuteReader();
        reader.Read();
        var number = reader["number"];
        var date = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date"));
        var dueDate = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("due_date"));
        var vatRate = (short)reader["vat_rate"];
        var subtotal = (long)reader["subtotal"];
        var vatAmount = (long)reader["vat_amount"];
        var total = (long)reader["total"];
        var supplierName = reader["supplier_name"];
        var supplierAddress = reader["supplier_address"];
        var supplierVatNumber = reader["supplier_vat_number"];
        var supplierIban = reader["supplier_iban"];
        var clientName = reader["client_name"];
        var clientAddress = reader["client_address"];
        var clientVatNumber = reader["client_vat_number"];
        var paid = (bool)reader["paid"];

        DateOnly? paidDate;

        if (!reader.IsDBNull(reader.GetOrdinal("paid_date")))
        {
            paidDate = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("paid_date"));
        }
        else
        {
            paidDate = null;
        }

        return media.With("Id", id)
                    .With("Client", clientName)
                    .With("Number", number)
                    .With("Date", date)
                    .With("Due date", dueDate)
                    .With("Subtotal", subtotal)
                    .With("VAT amount", vatAmount)
                    .With("Total", total)
                    .With("Paid", paid)
                    .With("Paid on", paidDate);
    }

    public void MarkPaid(DateOnly paidDate)
    {
        if (Nonexistent()) throw new Exception("Nonexistent invoice.");
        if (Paid()) throw new Exception("Cannot mark paid invoice as paid again.");

        using var cmd = pgDataSource.CreateCommand("UPDATE invoices SET paid = true, paid_date = $1 WHERE id = $2");
        cmd.Parameters.AddWithValue(paidDate);
        cmd.Parameters.AddWithValue(id);
        cmd.ExecuteNonQuery();
    }

    public void Send(ISmtpClient smtpClient)
    {
        var sql = """
            SELECT
                number,
                due_date,
                supplier_name,
                client_name,
                SUM(price * quantity::int) + ((SUM(price * quantity::int) * vat_rate) / 100) AS total,
                supplier_name,
                client_name,
                (SELECT email FROM clients WHERE id = client_id) AS client_email
            FROM
                invoices
            LEFT JOIN
                line_items ON invoices.id = line_items.invoice_id
            WHERE
                invoices.id = $1
            GROUP BY
                invoices.id
            """;

        using var cmd = pgDataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue(id);
        using var reader = cmd.ExecuteReader();
        reader.Read();
        var number = reader["number"];
        var dueDate = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("due_date"));
        var total = (long)reader["total"];
        var supplierName = reader["supplier_name"];
        var clientName = (string)reader["client_name"];
        var clientEmail = (string)reader["client_email"];

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(name: "John Doe", address: "invoice@intech.ee"));
        message.To.Add(MailboxAddress.Parse(clientEmail));
        message.Subject = $"Invoice no. {number} from {supplierName}";

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.Attachments.Add($"invoice_{number}_from_{supplierName}.pdf", Pdf().ToArray(), ContentType.Parse("application/pdf"));
        bodyBuilder.TextBody = new InterpolatedEmailTemplate(new InFileEmailTemplate("assets/email_template.txt"), dueDate, total, clientName).ToString();
        message.Body = bodyBuilder.ToMessageBody();

        smtpClient.Send(message);
    }

    bool Nonexistent()
    {
        var cmd = pgDataSource.CreateCommand("SELECT id FROM invoices WHERE id = $1");
        cmd.Parameters.AddWithValue(id);
        return cmd.ExecuteScalar() is null;
    }

    bool Paid()
    {
        var cmd = pgDataSource.CreateCommand("SELECT paid FROM invoices WHERE id = $1");
        cmd.Parameters.AddWithValue(id);
        return (bool)cmd.ExecuteScalar();
    }
}
