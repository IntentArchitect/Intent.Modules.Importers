# Intent SQL Schema Extractor - CLI Usage Guide

## Overview
The Intent SQL Schema Extractor has been refactored to use API-style commands with standardized JSON input/output for better programmatic integration.

## Commands

### 1. import-schema
Imports database schema into Intent package.

**Usage:**
```bash
Intent.SQLSchemaExtractor import-schema --payload '{"ConnectionString":"server=.;database=MyDB;trusted_connection=true","PackageFileName":"MyPackage.pkg"}'
```

**Payload Structure:**
```json
{
  "ConnectionString": "server=.;database=MyDB;trusted_connection=true",
  "PackageFileName": "MyPackage.pkg",
  "ImportFilterFilePath": "optional/path/to/filter.json",
  "ApplicationId": "optional-app-id",
  "EntityNameConvention": "SingularEntity",
  "TableStereotype": "WhenDifferent",
  "TypesToExport": ["Table", "View", "StoredProcedure", "Index"],
  "StoredProcedureType": "StoredProcedureElement",
  "RepositoryElementId": "optional-repo-id",
  "StoredProcNames": []
}
```

### 2. list-stored-procs
Returns a list of stored procedures in the database.

**Usage:**
```bash
Intent.SQLSchemaExtractor list-stored-procs --payload '{"ConnectionString":"server=.;database=MyDB;trusted_connection=true"}'
```

### 3. test-connection
Tests the connection to the database.

**Usage:**
```bash
Intent.SQLSchemaExtractor test-connection --payload '{"ConnectionString":"server=.;database=MyDB;trusted_connection=true"}'
```

### 4. retrieve-database-objects
Extracts database metadata (tables, views, stored procedures) as JSON.

**Usage:**
```bash
Intent.SQLSchemaExtractor retrieve-database-objects --payload '{"ConnectionString":"server=.;database=MyDB;trusted_connection=true"}'
```

## Response Format

All commands return a standardized JSON response:

```json
{
  "Result": {},
  "ResultType": "Intent.SQLSchemaExtractor.Models.SomeResultType",
  "Warnings": [],
  "Errors": []
}
```

### Success Response Example:
```json
{
  "Result": {
    "IsSuccessful": true,
    "DatabaseName": "MyDatabase",
    "ServerName": "MyServer"
  },
  "ResultType": "Intent.SQLSchemaExtractor.Models.ConnectionTestResult",
  "Warnings": [],
  "Errors": []
}
```

### Error Response Example:
```json
{
  "Result": {
    "IsSuccessful": false,
    "DatabaseName": "",
    "ServerName": ""
  },
  "ResultType": "Intent.SQLSchemaExtractor.Models.ConnectionTestResult",
  "Warnings": [],
  "Errors": [
    "Format of the initialization string does not conform to specification starting at index 0."
  ]
}
```

## Pretty Print Option

Add `--pretty-print` to any command for human-readable JSON output:

```bash
Intent.SQLSchemaExtractor test-connection --payload '{"ConnectionString":"server=.;database=MyDB;trusted_connection=true"}' --pretty-print
```

## Programmatic Usage

The single-line JSON output (default) is optimized for programmatic consumption:

```bash
# Capture result in variable
result=$(Intent.SQLSchemaExtractor test-connection --payload '{"ConnectionString":"..."}')

# Parse with jq or similar JSON tools
echo $result | jq '.Result.IsSuccessful'
```

## Migration from Old CLI

The old human-centric options have been replaced with JSON payloads:

**Old:**
```bash
Intent.SQLSchemaExtractor --connection-string "..." --package-file-name "..."
```

**New:**
```bash
Intent.SQLSchemaExtractor import-schema --payload '{"ConnectionString":"...","PackageFileName":"..."}'
