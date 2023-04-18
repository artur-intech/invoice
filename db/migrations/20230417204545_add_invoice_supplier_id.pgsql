ALTER TABLE invoices ADD supplier_id integer;
ALTER TABLE invoices ADD CONSTRAINT invoices_supplier_id_fkey FOREIGN KEY (supplier_id) REFERENCES suppliers(id);
