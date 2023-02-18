#!/bin/bash
set -e
createdb test
psql -U "$POSTGRES_USER" -d test -f /docker-entrypoint-initdb.d/schema.sql