ALTER TABLE clients ADD CONSTRAINT non_empty_clients_address CHECK (string_non_empty(address));
