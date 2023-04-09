ALTER TABLE clients DROP CONSTRAINT clients_email_check;
ALTER TABLE clients ADD CONSTRAINT non_empty_clients_email CHECK (email = '' IS NOT TRUE);
