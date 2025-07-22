using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using DatabaseSchemaReader.DataSchema;
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

    protected override string GetNormalizedDataTypeString(DataType? dataType, string dbDataType)
    {
        if (dbDataType == "jsonb")
        {
            return "string";
        }

        if (dbDataType == "bytea")
        {
            return "binary";
        }

        return base.GetNormalizedDataTypeString(dataType, dbDataType);
    }

    /// <summary>
    /// Override system object detection to include PostgreSQL-specific system schemas
    /// </summary>
    protected override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name)) return true;

        var postgresSystemSchemas = new[]
        {
            "information_schema", "pg_catalog", "pg_toast", "pg_temp", "pg_toast_temp",
            "sys", "INFORMATION_SCHEMA" // Include generic system schemas too
        };
        var systemTables = new[] { "sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory" };

        return postgresSystemSchemas.Contains(schema, StringComparer.OrdinalIgnoreCase) ||
               systemTables.Contains(name, StringComparer.OrdinalIgnoreCase) ||
               name.StartsWith("pg_", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Override stored procedure extraction to handle PostgreSQL functions properly.
    /// 
    /// CUSTOM IMPLEMENTATION REQUIRED: DatabaseSchemaReader has a limitation where it cannot
    /// properly handle PostgreSQL function overloads (functions with the same name but different
    /// parameter signatures). This causes extraction to fail or return incorrect results when
    /// overloaded functions are present in the database.
    /// 
    /// This implementation directly queries PostgreSQL system catalogs (pg_proc, pg_namespace, pg_type)
    /// to extract function information and properly distinguish between overloaded functions using
    /// their unique OIDs (Object Identifiers).
    /// </summary>
    protected override async Task<List<StoredProcedureSchema>> ExtractStoredProceduresAsync(
        DatabaseSchemaReader.DataSchema.DatabaseSchema databaseSchema,
        ImportFilterService importFilterService,
        DbConnection connection)
    {
        var storedProcedures = new List<StoredProcedureSchema>();

        // Use direct SQL queries to PostgreSQL system catalogs to handle function overloads properly
        // This bypasses DatabaseSchemaReader's limitation with overloaded functions
        var functions = await ExtractPostgreSQLFunctionsAsync(connection, importFilterService);

        var progressOutput = ConsoleOutput.CreateSectionProgress("Stored Procedures", functions.Count);

        foreach (var function in functions)
        {
            progressOutput.OutputNext($"{function.Schema}.{function.Name}");

            var storedProcSchema = new StoredProcedureSchema
            {
                Name = function.Name,
                Schema = function.Schema,
                Parameters = function.Parameters,
                ResultSetColumns = await ExtractStoredProcedureResultSetAsync(function, connection)
            };

            storedProcedures.Add(storedProcSchema);
        }

        return storedProcedures;
    }

    /// <summary>
    /// Extract PostgreSQL functions directly from system catalogs to properly handle overloads.
    /// 
    /// REASON FOR CUSTOM IMPLEMENTATION:
    /// DatabaseSchemaReader fails to correctly handle PostgreSQL function overloads because it
    /// treats functions with the same name as duplicates, causing extraction errors or data loss.
    /// PostgreSQL allows multiple functions with the same name but different parameter signatures,
    /// which is a common and valid pattern.
    /// 
    /// This method queries pg_proc directly and uses the function OID to uniquely identify each
    /// function variant, ensuring all overloads are properly extracted and processed.
    /// 
    /// Additionally, this single query approach prevents "A command is already in progress" errors
    /// that would occur with nested database readers.
    /// </summary>
    private async Task<List<PostgreSQLFunction>> ExtractPostgreSQLFunctionsAsync(DbConnection connection, ImportFilterService importFilterService)
    {
        var functionsDict = new Dictionary<uint, PostgreSQLFunction>();

        // Single comprehensive query to get all functions with their parameters in one round trip
        // This avoids DatabaseSchemaReader's issues with function overloads and prevents concurrent reader errors
        const string sql =
            """
            SELECT 
                p.oid as function_oid,
                n.nspname as schema_name,
                p.proname as function_name,
                p.pronargs as num_args,
                p.proargnames as arg_names,
                p.proargmodes as arg_modes,
                p.prorettype as return_type,
                p.proretset as returns_set,
                p.prokind as proc_kind,
                t.typname as return_type_name,
                -- Parameter information
                pt.typname as param_type_name,
                u.ordinality as param_position,
                CASE 
                    WHEN p.prokind = 'f' THEN 'function'
                    WHEN p.prokind = 'p' THEN 'procedure'
                    WHEN p.prokind = 'a' THEN 'aggregate'
                    WHEN p.prokind = 'w' THEN 'window'
                    ELSE 'unknown'
                END as routine_type
            FROM pg_proc p
            JOIN pg_namespace n ON p.pronamespace = n.oid
            JOIN pg_type t ON p.prorettype = t.oid
            LEFT JOIN unnest(p.proargtypes::oid[]) WITH ORDINALITY as u(type_oid, ordinality) ON true
            LEFT JOIN pg_type pt ON pt.oid = u.type_oid
            WHERE n.nspname NOT IN ('information_schema', 'pg_catalog', 'pg_toast', 'pg_temp', 'pg_toast_temp')
              AND n.nspname NOT LIKE 'pg_temp_%'
              AND n.nspname NOT LIKE 'pg_toast_temp_%'
              AND p.prokind IN ('f', 'p') -- functions and procedures only
            ORDER BY n.nspname, p.proname, p.oid, u.ordinality
            """;

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var schema = reader["schema_name"].ToString() ?? "public";
            var functionName = reader["function_name"].ToString() ?? "";
            var functionOid = Convert.ToUInt32(reader["function_oid"]);

            // Apply filtering
            if (!importFilterService.ExportStoredProcedure(schema, functionName))
                continue;

            if (IsSystemObject(schema, functionName))
                continue;

            // Apply additional filtering if specific function names are specified
            if (importFilterService.GetStoredProcedureNames().Count > 0)
            {
                var routineLookup = new HashSet<string>(importFilterService.GetStoredProcedureNames(), StringComparer.OrdinalIgnoreCase);
                if (!routineLookup.Contains(functionName) && !routineLookup.Contains($"{schema}.{functionName}"))
                    continue;
            }

            // Get or create function entry
            if (!functionsDict.TryGetValue(functionOid, out var function))
            {
                var numArgs = Convert.ToInt32(reader["num_args"]);
                var argNamesArray = reader["arg_names"] as string[];
                var argModesArray = reader["arg_modes"] as short[];
                var returnTypeName = reader["return_type_name"].ToString() ?? "void";
                var returnsSet = Convert.ToBoolean(reader["returns_set"]);

                function = new PostgreSQLFunction
                {
                    Oid = functionOid, // Unique identifier to distinguish overloads
                    Schema = schema,
                    Name = functionName, // Original function name (may have duplicates for overloads)
                    OriginalName = functionName, // Keep original name for display
                    Parameters = new List<StoredProcedureParameterSchema>(),
                    ReturnTypeName = returnTypeName,
                    ReturnsSet = returnsSet,
                    ArgNames = argNamesArray, // PostgreSQL function argument names
                    ArgModes = argModesArray // PostgreSQL function argument modes (in/out/inout)
                };

                functionsDict[functionOid] = function;
            }

            // Add parameter if it exists
            var paramTypeName = reader["param_type_name"]?.ToString();
            var paramPosition = reader["param_position"];

            if (!string.IsNullOrEmpty(paramTypeName) && paramPosition != DBNull.Value)
            {
                var position = Convert.ToInt32(paramPosition) - 1; // Convert to 0-based index
                var paramName = function.ArgNames != null && position < function.ArgNames.Length
                    ? function.ArgNames[position]
                    : $"param{position + 1}";

                // Determine if it's an output parameter (PostgreSQL uses modes: i=in, o=out, b=inout, v=variadic, t=table)
                var isOutput = false;
                if (function.ArgModes != null && position < function.ArgModes.Length)
                {
                    var mode = function.ArgModes[position];
                    isOutput = mode == 111 || mode == 98; // 'o' or 'b' (out or inout)
                }

                function.Parameters.Add(new StoredProcedureParameterSchema
                {
                    Name = paramName,
                    DataType = paramTypeName,
                    NormalizedDataType = NormalizePostgreSQLType(paramTypeName),
                    IsOutputParameter = isOutput,
                    MaxLength = null, // PostgreSQL system catalogs don't provide reliable length for function params
                    NumericPrecision = null,
                    NumericScale = null
                });
            }
        }

        return functionsDict.Values.ToList();
    }

    /// <summary>
    /// Extract result set for PostgreSQL function using the existing analyzer.
    /// 
    /// Note: We use the original function name here because the analyzer needs to work with
    /// the actual function name as it appears in PostgreSQL, not our internal unique identifier.
    /// </summary>
    private async Task<List<ResultSetColumnSchema>> ExtractStoredProcedureResultSetAsync(
        PostgreSQLFunction function,
        DbConnection connection)
    {
        var analyzer = CreateStoredProcedureAnalyzer(connection);
        return await analyzer.AnalyzeResultSetAsync(function.OriginalName, function.Schema, function.Parameters);
    }

    /// <summary>
    /// Maps PostgreSQL data types to fundamental types for Intent type mapping
    /// </summary>
    private static string NormalizePostgreSQLType(string? dataTypeName)
    {
        if (string.IsNullOrEmpty(dataTypeName))
            return "unknown";

        return dataTypeName.ToLowerInvariant().Trim() switch
        {
            // String types
            "varchar" or "text" or "char" or "bpchar" or "name" or "json" or "jsonb" or "xml" => "string",

            // Integer types
            "int4" or "integer" or "serial" => "int",
            "int8" or "bigint" or "bigserial" => "long",
            "int2" or "smallint" or "smallserial" => "short",

            // Decimal/Float types
            "numeric" or "decimal" or "money" or "float4" or "real" or "float8" or "double precision" => "decimal",

            // Boolean types
            "bool" or "boolean" => "bool",

            // Date/Time types
            "timestamp" or "timestamptz" => "datetime",
            "date" => "date",
            "time" or "timetz" or "interval" => "time",

            // UUID/GUID types
            "uuid" => "guid",

            // Binary types
            "bytea" => "binary",

            // PostgreSQL specific types that don't have direct equivalents - map to string
            "inet" or "cidr" or "macaddr" => "string", // Network types as strings
            "point" or "line" or "lseg" or "box" or "path" or "polygon" or "circle" => "string", // Geometric types as strings
            "bit" or "varbit" => "string", // Bit types as strings

            // Fallback for unknown types
            _ => "string"
        };
    }

    /// <summary>
    /// Enhanced foreign key extraction with proper referenced column names using PostgreSQL system catalogs
    /// </summary>
    protected override async Task<List<ForeignKeyColumnSchema>> ExtractForeignKeyColumnsAsync(DatabaseConstraint foreignKey, DbConnection connection)
    {
        var columns = new List<ForeignKeyColumnSchema>();

        if (string.IsNullOrEmpty(foreignKey.Name))
        {
            // Fallback to base implementation if no constraint name
            return await base.ExtractForeignKeyColumnsAsync(foreignKey, connection);
        }

        const string sql =
            """
            SELECT 
                kcu.column_name AS source_column,
                ccu.column_name AS referenced_column
            FROM information_schema.key_column_usage kcu
            INNER JOIN information_schema.constraint_column_usage ccu 
                ON kcu.constraint_name = ccu.constraint_name
                AND kcu.constraint_schema = ccu.constraint_schema
            WHERE kcu.constraint_name = @constraintName
              AND kcu.constraint_schema = @schemaName
            ORDER BY kcu.ordinal_position;
            """;

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var constraintNameParam = command.CreateParameter();
        constraintNameParam.ParameterName = "@constraintName";
        constraintNameParam.Value = foreignKey.Name;
        command.Parameters.Add(constraintNameParam);

        var schemaNameParam = command.CreateParameter();
        schemaNameParam.ParameterName = "@schemaName";
        schemaNameParam.Value = foreignKey.SchemaOwner ?? "public";
        command.Parameters.Add(schemaNameParam);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var sourceColumn = reader["source_column"]?.ToString() ?? "";
            var referencedColumn = reader["referenced_column"]?.ToString() ?? "";

            var columnSchema = new ForeignKeyColumnSchema
            {
                Name = sourceColumn,
                ReferencedColumnName = referencedColumn
            };

            columns.Add(columnSchema);
        }

        // If we didn't get any results, fall back to base implementation
        if (columns.Count == 0)
        {
            return await base.ExtractForeignKeyColumnsAsync(foreignKey, connection);
        }

        return columns;
    }

    /// <summary>
    /// Enhanced index extraction with PostgreSQL-specific metadata
    /// </summary>
    protected override async Task<List<IndexSchema>> ExtractTableIndexesAsync(DatabaseTable table, ImportFilterService importFilterService, DbConnection connection)
    {
        var indexes = new List<IndexSchema>();

        if (!importFilterService.ExportIndexes())
        {
            return indexes;
        }

        // Use a simplified query that works across all PostgreSQL versions
        const string sql =
            """
            SELECT 
                i.indexname AS index_name,
                i.indexdef AS index_definition,
                ix.indisunique AS is_unique,
                ix.indisclustered AS is_clustered,
                ix.indpred IS NOT NULL AS has_filter,
                CAST(ix.indpred AS text) AS filter_definition,
                STRING_AGG(
                    CASE 
                        WHEN a.attname IS NOT NULL THEN a.attname
                        ELSE 'expression'
                    END,
                    ','
                    ORDER BY array_position(ix.indkey, a.attnum)
                ) AS column_names,
                STRING_AGG(
                    CASE 
                        WHEN ix.indoption[array_position(ix.indkey, a.attnum) - 1] & 1 = 1 THEN 'DESC'
                        ELSE 'ASC'
                    END,
                    ','
                    ORDER BY array_position(ix.indkey, a.attnum)
                ) AS sort_orders,
                array_length(ix.indkey, 1) AS key_column_count
            FROM pg_indexes i
            INNER JOIN pg_class c ON c.relname = i.tablename
            INNER JOIN pg_namespace n ON n.nspname = i.schemaname AND n.oid = c.relnamespace
            INNER JOIN pg_index ix ON ix.indexrelid = (
                SELECT oid FROM pg_class WHERE relname = i.indexname AND relnamespace = n.oid
            )
            LEFT JOIN pg_attribute a ON a.attrelid = c.oid 
                AND a.attnum = ANY(ix.indkey[0:array_length(ix.indkey,1)-1])
                AND a.attnum > 0
            WHERE i.schemaname = @schemaName
              AND i.tablename = @tableName
            GROUP BY i.indexname, i.indexdef, ix.indisunique, ix.indisclustered, 
                     ix.indpred, ix.indkey, ix.indoption
            ORDER BY i.indexname;
            """;

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var schemaParam = command.CreateParameter();
        schemaParam.ParameterName = "@schemaName";
        schemaParam.Value = table.SchemaOwner ?? "public";
        command.Parameters.Add(schemaParam);

        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@tableName";
        tableParam.Value = table.Name;
        command.Parameters.Add(tableParam);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var indexName = reader["index_name"]?.ToString() ?? "";
            var isUnique = Convert.ToBoolean(reader["is_unique"]);
            var isClustered = Convert.ToBoolean(reader["is_clustered"]);
            var hasFilter = Convert.ToBoolean(reader["has_filter"]);
            var filterDefinition = reader["filter_definition"]?.ToString();
            var columnNames = reader["column_names"]?.ToString()?.Split(',') ?? [];
            var sortOrders = reader["sort_orders"]?.ToString()?.Split(',') ?? [];

            var indexColumns = new List<IndexColumnSchema>();

            for (int i = 0; i < columnNames.Length; i++)
            {
                if (string.IsNullOrEmpty(columnNames[i]) || columnNames[i] == "expression")
                    continue;

                var isDescending = i < sortOrders.Length && sortOrders[i] == "DESC";

                var columnSchema = new IndexColumnSchema
                {
                    Name = columnNames[i].Trim(),
                    IsDescending = isDescending,
                    IsIncluded = false // For now, we'll set this to false for compatibility
                };

                indexColumns.Add(columnSchema);
            }

            var indexSchema = new IndexSchema
            {
                Name = indexName,
                IsUnique = isUnique,
                IsClustered = isClustered,
                HasFilter = hasFilter,
                FilterDefinition = filterDefinition,
                Columns = indexColumns
            };

            indexes.Add(indexSchema);
        }

        // If we didn't get any results from our custom query, fall back to base implementation
        if (indexes.Count == 0)
        {
            return await base.ExtractTableIndexesAsync(table, importFilterService, connection);
        }

        return indexes;
    }

    /// <summary>
    /// Internal class to represent PostgreSQL functions with overload support.
    /// 
    /// This class exists because DatabaseSchemaReader's function representation doesn't
    /// properly handle PostgreSQL function overloads. We need to track additional metadata
    /// like OID, argument names, and argument modes that are specific to PostgreSQL.
    /// </summary>
    /// <remarks>
    /// Once DatabaseSchemaReader is able to support Functions in PostgreSQL, this class can be removed.
    /// </remarks>
    private class PostgreSQLFunction
    {
        public uint Oid { get; set; }
        public string Schema { get; set; } = "";
        public string Name { get; set; } = ""; // Unique name with OID
        public string OriginalName { get; set; } = ""; // Original function name
        public List<StoredProcedureParameterSchema> Parameters { get; set; } = [];
        public string ReturnTypeName { get; set; } = "";
        public bool ReturnsSet { get; set; }
        public string[]? ArgNames { get; set; }
        public short[]? ArgModes { get; set; }
    }
}