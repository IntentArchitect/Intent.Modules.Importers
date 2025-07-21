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
            var sql = """
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

            using var command = _connection.CreateCommand();
            command.CommandText = sql;

            var procParam = command.CreateParameter();
            procParam.ParameterName = "@procedureName";
            procParam.Value = procedureName;
            command.Parameters.Add(procParam);

            var schemaParam = command.CreateParameter();
            schemaParam.ParameterName = "@schema";
            schemaParam.Value = schema ?? "public";
            command.Parameters.Add(schemaParam);

            using var reader = await command.ExecuteReaderAsync();
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
                        DataType = MapPostgreSQLDataType(returnType),
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
                        DataType = MapPostgreSQLDataType(returnType),
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