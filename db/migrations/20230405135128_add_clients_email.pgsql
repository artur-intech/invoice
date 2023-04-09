ALTER TABLE clients ADD COLUMN email varchar CHECK (email = '' IS NOT FALSE) NULL;

-- To be run after a data migration
-- ALTER TABLE invoices ALTER COLUMN email SET NOT NULL;
