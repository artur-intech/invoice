ALTER TABLE suppliers ADD COLUMN email varchar CHECK (string_non_empty(email));
