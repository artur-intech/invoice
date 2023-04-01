SELECT
invoices.*,
 SUM(price * quantity::int) AS subtotal,
(SUM(price * quantity::int) * vat_rate) / 100 AS vat_amount,
SUM(price * quantity::int) + ((SUM(price * quantity::int) * vat_rate) / 100) AS total
FROM
invoices
LEFT JOIN
line_items ON invoices.id = line_items.invoice_id
-- WHERE
-- invoices.id = 10
GROUP BY
invoices.id
