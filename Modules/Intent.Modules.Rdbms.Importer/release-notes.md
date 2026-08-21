### Version 1.0.20

- Fixed: Issue where a manual change to a table's return type was reverted on reimport.
- Fixed: Issue where a table/view column's or stored procedure result attribute's `IsNullable` value would be reverted to match the database on re-import
 
### Version 1.0.19

- Fixed: Issue where the stored procedure import dialog's Connection String and Database Type fields were shown as required whenever **Remember Settings** was set to anything other than "Inherit Database Settings".
- Fixed: Issue where a stored procedure or operation that previously had no return type would incorrectly retain `IsCollection`/`IsNullable` as unset instead of being given their normal defaults the first time it received a return type.
- Fixed: Issue where a stored procedure's return type mapping was kept even after the database stopped reporting a result set for it (and detection had not simply failed); the return type and `IsCollection` are now cleared in that case.
- Fixed: Issue where element-to-element mappings for a stored procedure's result set were not removed on re-import once the corresponding type was no longer present in the updated result set.

### Version 1.0.18

- Fixed: Issue where an import using the "Team-shared metadata (sanitized connection string, no password)" setting would report a `PlatformNotSupportedException` for `Microsoft.Data.SqlClient`, even though the import itself had succeeded. Stripping the password no longer relies on a SQL Server specific connection string builder, which also means it now works for PostgreSQL connection strings.
- Fixed: Issue where a failure to remember the import settings would be reported as an import failure, and could mask the real error when the import itself was what had failed.

- Fixed: Issue where stored procedure elements/operations and their parameters would always have `Intent.EntityFrameworkCore.Repositories`-owned stereotypes applied, and stored procedure names would always be reformatted into C# identifiers (e.g. stripping `sp_`/`prc`/`proc` prefixes and PascalCasing), even when that module isn't installed in the target application. These behaviors are now gated on whether `Intent.EntityFrameworkCore.Repositories` is installed; when it isn't, the raw database stored procedure name is used and no EF.Repositories stereotypes are written.

### Version 1.0.17

- Fixed: Issue where a stored procedure or operation's return type could be silently reset to void on re-import if the database provider was unable to determine the result set for that run (e.g. dynamic SQL, temp tables). The existing return type is now preserved when this happens, with a warning added to the import output.

### Version 1.0.16

- Fixed: Issue where association names would be incorrect if more than one association was present.
- Fixed: Issue where the schema was not correctly being set on table import.

### Version 1.0.15

- Fixed: Issue where stored procedure operations moved to different repositories would be duplicated on re-import instead of being updated in place. Now uses global ExternalReference matching to find operations across all repositories.
- Fixed: Issue where the `IsNullable` property on the return type of operations and stored procedure elements would be reset to false on re-import if it had been manually changed to true.

### Version 1.0.14

- Improvement: Return type for stored procedures and linked operations, can have the `IsCollection` changed in the designer without reimporting overwriting it.
- Fixed: Issue where DataContract was created with "Results" attribute incorrectly when importing a stored procedure.
- Fixed: Issue where Stored Procedure return mapping wasn't setting `IsCollection` correctly.
- Fixed: Issue where OUT parameters on stored procedures were inaccurately mapped to an operation parameter.

### Version 1.0.13

- Improvement: Better detection of associations to prevent duplicate associations being created.
- Fixed: Issue where duplicate stored procedures where created on import if in a folder.
- Fixed: Error when importing stored procedure which used a temp table, and response DataContract could not be determined.

### Version 1.0.12

- Improvement: Added `Stored Procedure Operation mapped to Element` option for stored procedure imports. Will explicitly create the mapping from operation to stored procedure.
- Improvement: Updated referenced `Microsoft.Data.SqlClient` package version.

### Version 1.0.11

- Fixed: User-Defined Table Type stereotype settings applied to Types for Stored Procedures.

### Version 1.0.10

- Fixed: Importing Stored procedures with `OUT` parameters will generate Operations that invoke underlying Stored Procedures and return with an adequately mapped return type where the OUT parameter will be stored.

### Version 1.0.9

- Improvement: Added ProjectUrl link.

### Version 1.0.8

- Improvement: Updated module documentation to use centralized documentation site.
- Fixed: Repository Importer did not register the selected stored procedures from the import dialog.
- Fixed: Duplicate foreign keys with different names now correctly handled by preferring explicitly named FKs over auto-generated ones.

### Version 1.0.7

- Improvement: Importer now prompts to save unsaved changes before running.
- Fixed: Importing a table where a column with a PK also has a FK to another table's PK column should only be a 1 -> 1 and shouldn't import the FK too.
- Fixed: Importing "As-is" will now not remove underscores from table/column names.
- Fixed: Association SpecializationType and Name for source end wasn't being set correctly.

### Version 1.0.6

- Fixed: Remove deleted database associations only for selected `Include` tables.

### Version 1.0.5

- Improvement: Remove deleted database attributes and associations.
- Improvement: Preserve user-specified attribute types.

### Version 1.0.4

- Improvement: Introducing `Attribute Name Convention` for how Column names translates into Entity attribute names.
- Improvement: Added a check to make sure Import Filter File will write to location successfully.
- Improvement: Import Type default is `Include`.
- Fixed: The `Filter Type` will now be remembered when the next Import has to happen.
- Fixed: Incorrectly looked for duplicate associations.
- Fixed: Stored procedure `Name In Schema` is correctly set.

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

