using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Microsoft.Data.SqlClient;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

/// <summary>
/// SQL Server-specific stored procedure analyzer using T-SQL queries instead of SMO
/// Migrated from SqlServerStoredProcExtractor to work with DatabaseSchemaReader
/// </summary>
internal class SqlServerStoredProcedureAnalyzer : IStoredProcedureAnalyzer
{
    private readonly DbConnection _connection;

    public SqlServerStoredProcedureAnalyzer(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<ResultSetColumnSchema>> AnalyzeResultSetAsync(string procedureName, string schema, IEnumerable<StoredProcedureParameterSchema> parameters)
    {
        var resultColumns = new List<ResultSetColumnSchema>();

        try
        {
            // Use sp_describe_first_result_set to get result set metadata
            // This is the T-SQL equivalent of the SMO approach from SqlServerStoredProcExtractor
            var sql = $"""
                EXEC sp_describe_first_result_set 
                    @tsql = N'EXEC [{schema}].[{procedureName}]',
                    @params = N'',
                    @browse_information_mode = 1
                """;

            await using var command = _connection.CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = 60;

            await using var reader = await command.ExecuteReaderAsync();
            var dataTable = new DataTable();
            dataTable.Load(reader);

            // Group by source table to handle table identification (migrated from SqlServerStoredProcExtractor logic)
            var keyGroupedSourceRows = dataTable.Rows.Cast<DataRow>()
                .GroupBy(BuildKey)
                .ToArray();

            // Get table IDs for source tables (migrated approach)
            var tableIdLookup = new Dictionary<DataRow, int?>();
            
            foreach (var group in keyGroupedSourceRows)
            {
                if (!string.IsNullOrEmpty(group.Key))
                {
                    var tableIdSql = $"SELECT OBJECT_ID('{group.Key}') AS TableID";
                    var tableId = await GetTableIdAsync(tableIdSql);
                    
                    foreach (var row in group)
                    {
                        tableIdLookup[row] = tableId;
                    }
                }
                else
                {
                    foreach (var row in group)
                    {
                        tableIdLookup[row] = null;
                    }
                }
            }

            // Convert DataTable rows to ResultSetColumnSchema (migrated from SqlServerStoredProcExtractor.ResultSetColumn)
            foreach (DataRow row in dataTable.Rows)
            {
                var columnSchema = new ResultSetColumnSchema
                {
                    Name = GetStringValue(row, "name") ?? "",
                    DataType = SanitizeSystemTypeName(GetStringValue(row, "system_type_name")),
                    NormalizedDataType = NormalizeDataType(GetStringValue(row, "system_type_name")),
                    IsNullable = GetBoolValue(row, "is_nullable"),
                    MaxLength = null, // sp_describe_first_result_set doesn't provide reliable length info
                    NumericPrecision = null, // sp_describe_first_result_set doesn't provide reliable precision info  
                    NumericScale = null // sp_describe_first_result_set doesn't provide reliable scale info
                };

                resultColumns.Add(columnSchema);
            }
        }
        catch (Exception ex)
        {
            ConsoleOutput.WarnOutput($"Could not extract result set for stored procedure {schema}.{procedureName}: {ex.Message}");
        }

        return resultColumns;
    }

    /// <summary>
    /// Get table ID for source table identification (migrated from SqlServerStoredProcExtractor)
    /// </summary>
    private async Task<int?> GetTableIdAsync(string sql)
    {
        try
        {
            await using var command = _connection.CreateCommand();
            command.CommandText = sql;
            var result = await command.ExecuteScalarAsync();
            return result is not DBNull ? (int?)result : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Build key for grouping source rows (migrated from SqlServerStoredProcExtractor.BuildKey)
    /// </summary>
    private static string BuildKey(DataRow row)
    {
        var parts = new List<string>();
        
        var db = GetStringValue(row, "source_database");
        if (!string.IsNullOrEmpty(db))
            parts.Add(db);

        var schema = GetStringValue(row, "source_schema");
        if (!string.IsNullOrEmpty(schema))
            parts.Add(schema);

        var table = GetStringValue(row, "source_table");
        if (!string.IsNullOrEmpty(table))
            parts.Add(table);

        return string.Join(".", parts);
    }

    /// <summary>
    /// Get string value from DataRow with null handling
    /// </summary>
    private static string? GetStringValue(DataRow row, string columnName)
    {
        return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value 
            ? row[columnName]?.ToString() 
            : null;
    }

    /// <summary>
    /// Get boolean value from DataRow with null handling
    /// </summary>
    private static bool GetBoolValue(DataRow row, string columnName)
    {
        return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value 
            && Convert.ToBoolean(row[columnName]);
    }

    /// <summary>
    /// Sanitize system type name (migrated from SqlServerStoredProcExtractor.ResultSetColumn.Sanitize)
    /// </summary>
    private static string SanitizeSystemTypeName(string? systemTypeName)
    {
        if (string.IsNullOrEmpty(systemTypeName))
            return "unknown";

        // Remove length/precision information like (255) or (18,2)
        var sanitizeRegex = new Regex(@"(\([^\)]+\))$", RegexOptions.Compiled);
        return sanitizeRegex.Replace(systemTypeName, string.Empty).ToLowerInvariant();
    }

    /// <summary>
    /// Normalize SQL Server data type to C# type (using SqlServerDataTypeMapper logic)
    /// </summary>
    private static string NormalizeDataType(string? systemTypeName)
    {
        if (string.IsNullOrEmpty(systemTypeName))
            return "unknown";

        var cleanType = SanitizeSystemTypeName(systemTypeName);
        
        return cleanType switch
        {
            // String types
            "varchar" or "nvarchar" or "char" or "nchar" or "text" or "ntext" or "sysname" or "xml" => "string",

            // Integer types
            "int" => "int",
            "bigint" => "long", 
            "smallint" => "short",
            "tinyint" => "byte",

            // Decimal/Float types
            "decimal" or "numeric" or "money" or "smallmoney" or "float" or "real" => "decimal",

            // Boolean types
            "bit" => "bool",

            // Date/Time types
            "datetime" or "datetime2" or "smalldatetime" => "datetime",
            "datetimeoffset" => "datetimeoffset",
            "date" => "date",
            "time" => "time",

            // UUID/GUID types
            "uniqueidentifier" => "guid",

            // Binary types
            "varbinary" or "binary" or "image" or "timestamp" => "binary",
            
            // Fallback
            _ => "string"
        };
    }
} 