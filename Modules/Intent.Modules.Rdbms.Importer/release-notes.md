### Version 1.0.4

- Improvement: Introducing `Attribute Name Convention` for how Column names translates into Entity attribute names.
- Fixed: The `Filter Type` will now be remembered when the next Import has to happen.

### Version 1.0.3

- Improvement: Switched to use a module task to show progress more explicitly.

### Version 1.0.2

- Fixed: Stereotypes not adequately applied to Tables.

### Version 1.0.1

- Improved: Foreign key handling in RdbmsSchemaAnnotator to manage metadata more effectively.
- Improved: Unified lookup helper for element retrieval with 3-level precedence.
- Improved: Enhance system object filtering for PostgreSQL and SQL Server.
- Improved: Stored procedure parameter (and result-set) handling.
- Improved: Error and warning messages for better clarity during import.
- Improved: SQL Server and PostgreSQL metadata extraction to handle more cases.
- Fixed: Filter JSON file didn't handle relative paths well.
- Fixed: Filter JSON fields not supported in UI will be retained.
- Fixed: StoredProcedureRepository is placed in the package level and not in the folder-schema level.
- Fixed: Text constraints applied to attributes will now correctly apply to either a Column stereotype type or a Text Constraint stereotype type.
- Fixed: Stored Procedure operations and parameters are now correctly decorated with the correct stereotype information.
- Fixed: Re-evaluate stereotypes for existing class elements after synchronization.

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

