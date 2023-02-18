DROP VIEW IF EXISTS all_invoices;

CREATE VIEW all_invoices AS
    SELECT
    i.*,
    sum(price * quantity) AS subtotal,
    sum((price * quantity) * vat_rate / 100) AS vat_amount,
    sum((price * quantity) + ((price * quantity) * vat_rate / 100)) AS total
    FROM
    invoices i
    JOIN line_items l ON (i.id = l.invoice_id)
    group by i.id