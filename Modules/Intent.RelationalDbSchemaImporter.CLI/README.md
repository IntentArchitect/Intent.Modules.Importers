# Intent Relational Database Schema Importer

This is an RPC-style backend tool for database schema extraction used by Intent Architect importer modules. It provides database schema extraction services for both SQL Server and PostgreSQL databases through JSON-based command interfaces.

## Purpose

This tool is **not intended for direct end-user interaction**. Instead, it serves as a backend service that Intent Architect importer modules communicate with to extract database schema information. The tool is separated into its own executable due to native library dependencies (`Microsoft.Data.SqlClient`) that cannot be included directly in Intent Architect modules.

## Architecture

The tool operates as an RPC-style service that:
- Accepts JSON payloads via command line arguments (`--payload` parameter)
- Executes database operations using provider-specific implementations
- Returns structured JSON responses with results or error information
- Supports real-time progress updates through stderr

## Supported Commands

### `import-schema`
Extracts complete database schema including tables, views, stored procedures, and relationships.

### `test-connection`
Validates database connectivity with the provided connection string.

### `list-stored-procedures`
Returns a list of stored procedures and functions in the database.

### `retrieve-database-objects`
Extracts metadata for database objects (tables, views, stored procedures) without full schema details.

## Supported Databases

- **SQL Server** - Full support including stored procedures, functions, and complex data types
- **PostgreSQL** - Full support including functions, procedures, and PostgreSQL-specific features

## Known Limitations

### Function/Procedure Overloads
DatabaseSchemaReader has a known bug when handling stored procedures or functions with the same name but different parameter signatures (overloads). This primarily affects PostgreSQL databases where function overloading is commonly used.

**Current Behavior:** When multiple overloads are detected, only the first occurrence is imported. Other overloads are filtered out to prevent import errors.

**Workaround:** If you need support for function overloads, this can be addressed with a custom PostgreSQL-specific implementation. Please report this as an issue if it affects your use case.

**Affected Scenarios:**
- PostgreSQL functions with multiple parameter signatures
- PostgreSQL procedures with overloaded definitions
- Any database where similar naming patterns exist

## Usage by Intent Architect Modules

Intent Architect importer modules (such as `Intent.Modules.SqlServerImporter`) use the `Intent.RelationalDbSchemaImporter.Runner.ImporterTool` class to invoke this CLI tool programmatically:

```csharp
// Set the tool directory containing the CLI executable
ImporterTool.SetToolDirectory(toolDirectory);

// Execute a command with JSON payload
var result = ImporterTool.Run<ImportSchemaResult>("import-schema", requestPayload);
```

The tool is typically deployed as part of Intent Architect module packages in the `content/tool` directory.

