### Version 1.0.1

- Improved: Foreign key handling in RdbmsSchemaAnnotator to manage metadata more effectively.
- Improved: Unified lookup helper for element retrieval with 3-level precedence.

### Version 1.0.0

- New Feature: Module release supporting both SQL Server and PostgreSQL database imports. This module replaces the previous `Intent.SqlServerImporter` module with expanded database support.
- New Feature: Module renamed from `Intent.SqlServerImporter` to `Intent.Rdbms.Importer` to reflect multi-database support.
- New Feature: Full PostgreSQL support including tables, views, indexes, foreign keys, and stored procedures (functions).
- New Feature: PostgreSQL-specific function parameter handling with support for argument modes (in/out/inout).
- New Feature: PostgreSQL data type mapping with support for PostgreSQL-specific types.
- New Feature: System schema filtering for PostgreSQL (excludes pg_catalog, information_schema, etc.).
- Improvement: Enhanced database provider architecture supporting multiple database types.
- Improvement: Unified import dialog supporting both SQL Server and PostgreSQL connection strings.
- Improvement: Enhanced stored procedure handling for PostgreSQL functions with overload support.
- Improvement: PostgreSQL dependency resolution for proper import ordering.
- Improvement: Comprehensive PostgreSQL index extraction including unique and partial indexes.

