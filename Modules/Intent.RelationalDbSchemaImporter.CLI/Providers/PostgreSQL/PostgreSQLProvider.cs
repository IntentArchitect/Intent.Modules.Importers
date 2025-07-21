using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Npgsql;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

/// <summary>
/// PostgreSQL database provider implementation
/// </summary>
internal class PostgreSQLProvider : BaseDatabaseProvider
{
    public override DatabaseType SupportedType => DatabaseType.PostgreSQL;

    protected override DbConnection CreateConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }

    protected override IDependencyResolver CreateDependencyResolver(DbConnection connection)
    {
        return new PostgreSQLDependencyResolver(connection);
    }

    protected override IStoredProcedureAnalyzer CreateStoredProcedureAnalyzer(DbConnection connection)
    {
        return new PostgreSQLStoredProcedureAnalyzer(connection);
    }

    protected override string GetDataTypeString(string? dataTypeName)
    {
        return MapPostgreSQLDataType(dataTypeName);
    }

    /// <summary>
    /// Override system object detection to include PostgreSQL-specific system schemas
    /// </summary>
    protected override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name)) return true;
        
        var postgresSystemSchemas = new[] { 
            "information_schema", "pg_catalog", "pg_toast", "pg_temp", "pg_toast_temp",
            "sys", "INFORMATION_SCHEMA" // Include generic system schemas too
        };
        var systemTables = new[] { "sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory" };
        
        return postgresSystemSchemas.Contains(schema, StringComparer.OrdinalIgnoreCase) ||
               systemTables.Contains(name, StringComparer.OrdinalIgnoreCase) ||
               name.StartsWith("pg_", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Override stored procedure extraction to handle PostgreSQL functions properly
    /// </summary>
    protected override async Task<List<StoredProcedureSchema>> ExtractStoredProceduresAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema, 
        ImportFilterService importFilterService,
        DbConnection connection)
    {
        var storedProcedures = new List<StoredProcedureSchema>();
        
        // PostgreSQL primarily uses functions instead of stored procedures
        var routines = new List<DatabaseSchemaReader.DataSchema.DatabaseStoredProcedure>();
        
        // Add functions (primary approach for PostgreSQL)
        if (databaseSchema.Functions != null)
        {
            var functionRoutines = databaseSchema.Functions
                .Where(func => !IsSystemObject(func.SchemaOwner, func.Name) && 
                              importFilterService.ExportStoredProcedure(func.SchemaOwner, func.Name))
                .Cast<DatabaseSchemaReader.DataSchema.DatabaseStoredProcedure>();
            
            routines.AddRange(functionRoutines);
        }
        
        // Add stored procedures if any exist (PostgreSQL 11+ has some stored procedure support)
        if (databaseSchema.StoredProcedures != null)
        {
            routines.AddRange(databaseSchema.StoredProcedures
                .Where(sp => !IsSystemObject(sp.SchemaOwner, sp.Name) && 
                            importFilterService.ExportStoredProcedure(sp.SchemaOwner, sp.Name)));
        }

        // Apply additional filtering if specific routine names are specified
        if (importFilterService.GetStoredProcedureNames().Count > 0)
        {
            var routineLookup = new HashSet<string>(importFilterService.GetStoredProcedureNames(), StringComparer.OrdinalIgnoreCase);
            routines = routines.Where(routine => 
                routineLookup.Contains(routine.Name) || 
                routineLookup.Contains($"{routine.SchemaOwner}.{routine.Name}"))
                .ToList();
        }

        foreach (var routine in routines)
        {
            var storedProcSchema = new StoredProcedureSchema
            {
                Name = routine.Name,
                Schema = routine.SchemaOwner,
                Parameters = ExtractStoredProcedureParameters(routine),
                ResultSetColumns = await ExtractStoredProcedureResultSetAsync(routine, connection)
            };

            storedProcedures.Add(storedProcSchema);
        }

        return storedProcedures;
    }

    /// <summary>
    /// Maps PostgreSQL data types to normalized names
    /// </summary>
    private string MapPostgreSQLDataType(string? dataTypeName)
    {
        if (string.IsNullOrEmpty(dataTypeName))
            return "unknown";

        return dataTypeName.ToLowerInvariant() switch
        {
            "character varying" => "varchar",
            "character" => "char",
            "text" => "text",
            "integer" => "int",
            "bigint" => "bigint",
            "smallint" => "smallint",
            "decimal" => "decimal",
            "numeric" => "numeric",
            "real" => "real",
            "double precision" => "float",
            "boolean" => "boolean",
            "timestamp without time zone" => "timestamp",
            "timestamp with time zone" => "timestamptz",
            "date" => "date",
            "time without time zone" => "time",
            "time with time zone" => "timetz",
            "uuid" => "uuid",
            "json" => "json",
            "jsonb" => "jsonb",
            "bytea" => "bytea",
            "inet" => "inet",
            "cidr" => "cidr",
            "macaddr" => "macaddr",
            "point" => "point",
            "line" => "line",
            "lseg" => "lseg",
            "box" => "box",
            "path" => "path",
            "polygon" => "polygon",
            "circle" => "circle",
            "bit" => "bit",
            "bit varying" => "varbit",
            "money" => "money",
            "interval" => "interval",
            "xml" => "xml",
            "serial" => "serial",
            "bigserial" => "bigserial",
            "smallserial" => "smallserial",
            _ => dataTypeName.ToLowerInvariant()
        };
    }
} 