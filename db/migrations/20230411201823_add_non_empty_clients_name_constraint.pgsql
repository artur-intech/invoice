ALTER TABLE clients ADD CONSTRAINT non_empty_clients_name CHECK (name = '' IS NOT TRUE);
