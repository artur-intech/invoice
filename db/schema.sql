--
-- PostgreSQL database dump
--

-- Dumped from database version 14.3 (Debian 14.3-1.pgdg110+1)
-- Dumped by pg_dump version 14.7 (Debian 14.7-1.pgdg110+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: string_non_empty(text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.string_non_empty(string text) RETURNS boolean
    LANGUAGE sql IMMUTABLE STRICT
    RETURN ((string = ''::text) IS NOT TRUE);


ALTER FUNCTION public.string_non_empty(string text) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: applied_migrations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.applied_migrations (
    id character varying(255) NOT NULL
);


ALTER TABLE public.applied_migrations OWNER TO postgres;

--
-- Name: clients; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.clients (
    id integer NOT NULL,
    name character varying NOT NULL,
    address character varying NOT NULL,
    vat_number character varying,
    email character varying NOT NULL,
    CONSTRAINT non_empty_clients_address CHECK (public.string_non_empty((address)::text)),
    CONSTRAINT non_empty_clients_email CHECK (public.string_non_empty((email)::text)),
    CONSTRAINT non_empty_clients_name CHECK (public.string_non_empty((name)::text))
);


ALTER TABLE public.clients OWNER TO postgres;

--
-- Name: clients_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.clients ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.clients_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: invoices; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.invoices (
    id integer NOT NULL,
    client_id integer NOT NULL,
    number character varying NOT NULL,
    date date NOT NULL,
    due_date date NOT NULL,
    vat_rate smallint NOT NULL,
    supplier_name character varying NOT NULL,
    supplier_address character varying NOT NULL,
    supplier_vat_number character varying,
    supplier_iban character varying NOT NULL,
    client_name character varying NOT NULL,
    client_address character varying NOT NULL,
    client_vat_number character varying,
    paid_date date,
    paid boolean DEFAULT false NOT NULL,
    CONSTRAINT later_invoice_due_date CHECK ((due_date >= date)),
    CONSTRAINT nonnegative_invoice_vat_rate CHECK ((vat_rate >= 0))
);


ALTER TABLE public.invoices OWNER TO postgres;

--
-- Name: invoices_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.invoices ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.invoices_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: line_items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.line_items (
    id integer NOT NULL,
    invoice_id integer NOT NULL,
    name character varying NOT NULL,
    price smallint NOT NULL,
    quantity smallint NOT NULL,
    CONSTRAINT line_items_quantity_check CHECK ((quantity > 0))
);


ALTER TABLE public.line_items OWNER TO postgres;

--
-- Name: line_items_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.line_items ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.line_items_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: suppliers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.suppliers (
    id integer NOT NULL,
    name character varying NOT NULL,
    address character varying NOT NULL,
    vat_number character varying,
    iban character varying NOT NULL,
    email character varying NOT NULL,
    CONSTRAINT suppliers_email_check CHECK (public.string_non_empty((email)::text))
);


ALTER TABLE public.suppliers OWNER TO postgres;

--
-- Name: suppliers_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.suppliers ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.suppliers_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Data for Name: applied_migrations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.applied_migrations (id) FROM stdin;
20230317131943_add_invoices_paid_date
20230318170111_create_invoice_state
20230320133338_add_invoices_state
20230321142337_add_invoices_paid
20230321142810_drop_invoices_state
20230321143825_drop_invoice_state
20230405135128_add_clients_email
20230405161039_fix_clients_email_constraint
20230411112222_change_clients_email_to_not_null
20230411201823_add_non_empty_clients_name_constraint
20230412112817_add_string_non_empty_function
20230412114243_use_string_non_empty_function_in_clients
20230413131250_add_non_empty_clients_address_constraint
20230417135856_add_suppliers_email
20230417160754_change_suppliers_email_to_not_null
\.


--
-- Name: clients_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.clients_id_seq', 1, true);


--
-- Name: invoices_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.invoices_id_seq', 1, true);


--
-- Name: line_items_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.line_items_id_seq', 1, true);


--
-- Name: suppliers_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.suppliers_id_seq', 1, true);


--
-- Name: applied_migrations applied_migrations_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.applied_migrations
    ADD CONSTRAINT applied_migrations_id_key UNIQUE (id);


--
-- Name: clients clients_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clients
    ADD CONSTRAINT clients_pkey PRIMARY KEY (id);


--
-- Name: invoices invoices_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT invoices_pkey PRIMARY KEY (id);


--
-- Name: line_items line_items_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.line_items
    ADD CONSTRAINT line_items_pkey PRIMARY KEY (id);


--
-- Name: suppliers suppliers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers
    ADD CONSTRAINT suppliers_pkey PRIMARY KEY (id);


--
-- Name: clients uniq_client_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clients
    ADD CONSTRAINT uniq_client_name UNIQUE (name);


--
-- Name: invoices uniq_invoice_number; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT uniq_invoice_number UNIQUE (number);


--
-- Name: suppliers uniq_supplier_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers
    ADD CONSTRAINT uniq_supplier_name UNIQUE (name);


--
-- Name: invoices invoices_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT invoices_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.clients(id);


--
-- Name: line_items line_items_invoice_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.line_items
    ADD CONSTRAINT line_items_invoice_id_fkey FOREIGN KEY (invoice_id) REFERENCES public.invoices(id);


--
-- PostgreSQL database dump complete
--

