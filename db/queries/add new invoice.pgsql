-- INSERT INTO invoices(
--     client_id, 
--     number, 
--     date, 
--     due_date, 
--     supplier_name, 
--     supplier_address, 
--     supplier_vat_number,
--     supplier_iban, 
--     customer_name, 
--     customer_address, 
--     customer_vat_number)
-- SELECT 
--     1,
--     substr(md5(random()::text), 0, 10),
--     CURRENT_DATE, 
--     CURRENT_DATE, 
--     s.name, 
--     s.address, 
--     s.vat_number,
--     s.iban, 
--     c.name, 
--     c.address, 
--     c.vat_number
-- FROM 
--     suppliers s, customers c
-- WHERE 
--     s.id = 1 AND c.id = 1
-- RETURNING id;

INSERT INTO invoices(
                   client_id, 
                   number, 
                   date, 
                   due_date, 
                   vat_rate, 
                   supplier_name, 
                   supplier_address, 
                   supplier_vat_number,
                   supplier_iban, 
                   client_name, 
                   client_address, 
                   client_vat_number)
                   SELECT
                   1, 
                   '20220728103003200',
                   CURRENT_DATE, 
                   CURRENT_DATE + INTERVAL '10 days',
                   0, 
                   s.name, 
                   s.address, 
                   s.vat_number,
                   s.iban, 
                   c.name, 
                   c.address, 
                   c.vat_number
                   FROM
                   suppliers s, clients c
                   WHERE
                   s.id = 1 AND c.id = 1
                   RETURNING id;
