using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System;
using System.Linq;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

/// <summary>
/// PostgreSQL-specific stored procedure analyzer for functions
/// </summary>
internal class PostgreSQLStoredProcedureAnalyzer : IStoredProcedureAnalyzer
{
    private readonly DbConnection _connection;

    public PostgreSQLStoredProcedureAnalyzer(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<ResultSetColumnSchema>> AnalyzeResultSetAsync(string procedureName, string schema, IEnumerable<StoredProcedureParameterSchema> parameters)
    {
        var resultColumns = new List<ResultSetColumnSchema>();

        try
        {
            // PostgreSQL uses functions instead of traditional stored procedures
            // Query the PostgreSQL system catalogs to get function return type information
            var sql =
                """
                SELECT 
                    p.proname as function_name,
                    t.typname as return_type,
                    CASE 
                        WHEN p.proretset THEN 'table'
                        ELSE 'scalar'
                    END as return_kind,
                    p.prorettype,
                    ns.nspname as schema_name
                FROM pg_proc p
                JOIN pg_namespace ns ON p.pronamespace = ns.oid
                JOIN pg_type t ON p.prorettype = t.oid
                WHERE p.proname = @procedureName 
                  AND ns.nspname = @schema
                  AND p.prokind IN ('f', 'p') -- functions and procedures
                """;

            await using var command = _connection.CreateCommand();
            command.CommandText = sql;

            var procParam = command.CreateParameter();
            procParam.ParameterName = "@procedureName";
            procParam.Value = procedureName;
            command.Parameters.Add(procParam);

            var schemaParam = command.CreateParameter();
            schemaParam.ParameterName = "@schema";
            schemaParam.Value = schema ?? "public";
            command.Parameters.Add(schemaParam);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var returnType = reader["return_type"].ToString();
                var returnKind = reader["return_kind"].ToString();

                if (returnKind == "table")
                {
                    // For table-returning functions, we could query pg_get_function_result_name/type
                    // For now, create a generic result column
                    resultColumns.Add(new ResultSetColumnSchema
                    {
                        Name = "result",
                        DataType = returnType ?? "unknown", // Use raw PostgreSQL type
                        NormalizedDataType = NormalizePostgreSQLType(returnType),
                        IsNullable = true,
                        MaxLength = null,
                        NumericPrecision = null,
                        NumericScale = null
                    });
                }
                else if (returnType != "void")
                {
                    // Scalar returning function
                    resultColumns.Add(new ResultSetColumnSchema
                    {
                        Name = procedureName, // Function name as column name
                        DataType = returnType ?? "unknown", // Use raw PostgreSQL type
                        NormalizedDataType = NormalizePostgreSQLType(returnType),
                        IsNullable = true,
                        MaxLength = null,
                        NumericPrecision = null,
                        NumericScale = null
                    });
                }
            }
        }
        catch (Exception)
        {
            // If analysis fails, return empty result set
            // This is common for functions that don't return result sets or require specific parameters
        }

        return resultColumns;
    }

    /// <summary>
    /// Maps PostgreSQL data types to fundamental types for Intent type mapping
    /// </summary>
    private string NormalizePostgreSQLType(string? dataTypeName)
    {
        if (string.IsNullOrEmpty(dataTypeName))
            return "unknown";

        return dataTypeName.ToLowerInvariant().Trim() switch
        {
            // String types
            "character varying" or "character" or "text" or "json" or "jsonb" or "xml" => "string",

            // Integer types
            "integer" or "serial" => "int",
            "bigint" or "bigserial" => "long",
            "smallint" or "smallserial" => "short",

            // Decimal/Float types
            "decimal" or "numeric" or "money" or "real" or "double precision" => "decimal",

            // Boolean types
            "boolean" => "bool",

            // Date/Time types (without timezone)
            "timestamp without time zone" => "datetime",

            // Date/Time types (with timezone)
            "timestamp with time zone" => "datetimeoffset",

            // Date only
            "date" => "date",

            // Time only
            "time without time zone" or "time with time zone" or "interval" => "time",

            // UUID/GUID types
            "uuid" => "guid",

            // Binary types
            "bytea" => "binary",

            // PostgreSQL specific types that don't have direct equivalents - map to string
            "inet" or "cidr" or "macaddr" => "string", // Network types as strings
            "point" or "line" or "lseg" or "box" or "path" or "polygon" or "circle" => "string", // Geometric types as strings
            "bit" or "bit varying" => "string", // Bit types as strings

            // Fallback for unknown types
            _ => "string"
        };
    }
}