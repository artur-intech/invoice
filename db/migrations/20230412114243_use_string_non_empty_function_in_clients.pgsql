ALTER TABLE clients DROP CONSTRAINT non_empty_clients_email;
ALTER TABLE clients DROP CONSTRAINT non_empty_clients_name;
ALTER TABLE clients ADD CONSTRAINT non_empty_clients_email CHECK (string_non_empty(email));
ALTER TABLE clients ADD CONSTRAINT non_empty_clients_name CHECK (string_non_empty(name));
