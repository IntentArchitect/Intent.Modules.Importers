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
        
        // Use sp_describe_first_result_set to get result set metadata
        // This is the T-SQL equivalent of the SMO approach from SqlServerStoredProcExtractor
        var sql =
            $"""
             EXEC sp_describe_first_result_set 
                 @tsql = N'EXEC [{schema}].[{procedureName}]',
                 @params = N'',
                 @browse_information_mode = 1
             """;
        // @browse_information_mode = 1 -- fetches the source tables

        await using var command = _connection.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = 60;

        await using var reader = await command.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(reader);

        var filteredRows = dataTable.Rows.Cast<DataRow>().Where(p => GetBoolValue(p, "is_hidden") == false).ToList();
        
        // Group by source table to handle table identification (migrated from SqlServerStoredProcExtractor logic)
        var keyGroupedSourceRows = filteredRows
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

        var anonymousColumnCounter = 0;
        
        foreach (var row in filteredRows)
        {
            var columnSchema = new ResultSetColumnSchema
            {
                Name = GetStringValue(row, "name") ?? $"Column{++anonymousColumnCounter}",
                DbDataType = SanitizeSystemTypeName(GetStringValue(row, "system_type_name")),
                LanguageDataType = NormalizeDataType(GetStringValue(row, "system_type_name")),
                SourceSchema = GetStringValue(row, "source_schema"),
                SourceTable =  GetStringValue(row, "source_table"),
                IsNullable = GetBoolValue(row, "is_nullable"),
                MaxLength = GetMaxLengthFromDbType(row, "system_type_name"),
                NumericPrecision = GetIntValue(row, "precision"),  
                NumericScale = GetIntValue(row, "scale")
            };

            resultColumns.Add(columnSchema);
        }

        return resultColumns;
    }

    private static readonly Regex ValueInParenthesesRegex = new Regex(@"\(([^\)]+)\)", RegexOptions.Compiled);
    private static int? GetMaxLengthFromDbType(DataRow row, string columnName)
    {
        // If the type is like "varchar(255)" we want to extract the number
        if (!row.Table.Columns.Contains(columnName))
            return null;
        
        object? value = row[columnName];
        
        if (value == null || value == DBNull.Value)
            return null;
        
        if (value is string strValue)
        {
            var match = ValueInParenthesesRegex.Match(strValue);
            if (!match.Success)
            {
                return null;
            }
            if (match.Value.Equals("max", StringComparison.InvariantCultureIgnoreCase))
            {
                return -1;
            }
            if (int.TryParse(match.Groups[1].Value, out var length))
            {
                return length;
            }
        }

        return null;
    }

    private static int? GetIntValue(DataRow row, string columnName, bool zeroIsNull = true)
    {
        if (!row.Table.Columns.Contains(columnName))
            return null;

        object? value = row[columnName];

        if (value == null || value == DBNull.Value)
            return null;

        // Handle different possible types
        int? intResult = value switch
        {
            int intValue => intValue,
            byte byteValue => byteValue,
            short shortValue => shortValue,
            long longValue => (int)longValue, // Cast long to int if necessary
            string stringValue when int.TryParse(stringValue, out var result) => result,
            _ => null
        };

        if (intResult == 0 && zeroIsNull)
        {
            return null;
        }

        return intResult;
    }

    private static bool GetBoolValue(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName))
            return false;
        
        object? value = row[columnName];
    
        if (value == null || value == DBNull.Value)
            return false;
        
        // Handle different possible types
        return value switch
        {
            bool boolValue => boolValue,
            byte byteValue => byteValue != 0,
            int intValue => intValue != 0,
            string stringValue => bool.TryParse(stringValue, out var result) && result,
            _ => false
        };
    }
    
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

    private static string? GetStringValue(DataRow row, string columnName)
    {
        return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
            ? row[columnName]?.ToString()
            : null;
    }

    // Remove length/precision information like (255) or (18,2)
    private static readonly Regex ParenthesesWithValueRegex = new Regex(@"(\([^\)]+\))$", RegexOptions.Compiled);
    
    private static string SanitizeSystemTypeName(string? systemTypeName)
    {
        if (string.IsNullOrEmpty(systemTypeName))
            return "unknown";
        
        return ParenthesesWithValueRegex.Replace(systemTypeName, string.Empty).ToLowerInvariant();
    }

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