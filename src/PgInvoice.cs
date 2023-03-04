using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Svg.Converter;
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

        var pdfFilename = $"{supplierName}_invoice_{number}.pdf";
        var writer = new PdfWriter(pdfFilename);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);
        var helveticaBold = PdfFontFactory.CreateRegisteredFont("helvetica-bold");
        var dimgray = WebColors.GetRGBColor("dimgray");

        var intechLogo = SvgConverter.ConvertToImage(File.Open(Path.Join("assets", "intech_logo.svg"), FileMode.Open), pdf);

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

        document.Close();
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
        var number = (string)reader["number"];
        var date = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date"));
        var dueDate = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("due_date"));
        var vatRate = (short)reader["vat_rate"];
        var invoiceSubtotal = (long)reader["subtotal"];
        var invoiceVatAmount = (long)reader["vat_amount"];
        var total = (long)reader["total"];
        var supplierName = (string)reader["supplier_name"];
        var supplierAddress = (string)reader["supplier_address"];
        var supplierVatNumber = (string)reader["supplier_vat_number"];
        var supplierIban = (string)reader["supplier_iban"];
        var clientName = (string)reader["client_name"];
        var clientAddress = (string)reader["client_address"];
        var clientVatNumber = (string)reader["client_vat_number"];

        return media.With("Id", id)
                    .With("Client", clientName)
                    .With("Number", number)
                    .With("Date", date)
                    .With("Due date", dueDate)
                    .With("Total", total);
    }
}