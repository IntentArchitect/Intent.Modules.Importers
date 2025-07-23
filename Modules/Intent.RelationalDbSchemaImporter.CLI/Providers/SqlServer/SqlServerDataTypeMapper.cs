using System;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

internal class SqlServerDataTypeMapper : DefaultDataTypeMapper
{
    public override string GetDataTypeString(string? dataTypeName)
    {
        if (string.IsNullOrEmpty(dataTypeName))
            return "unknown";

        // Handle SQL Server MAX types specifically
        return dataTypeName.ToLowerInvariant() switch
        {
            "varbinary(max)" => "varbinary",
            "varchar(max)" => "varchar",
            "nvarchar(max)" => "nvarchar",
            _ => base.GetDataTypeString(dataTypeName) // Use base logic for standard types
        };
    }

    public override string GetNormalizedDataTypeString(DataType? dataType, string dbDataType)
    {
        // Handle SQL Server-specific data types first
        var normalizedType = NormalizeSqlServerDataType(dbDataType);
        if (normalizedType != "unknown")
        {
            return normalizedType;
        }

        // Fall back to base implementation for standard types
        return base.GetNormalizedDataTypeString(dataType, dbDataType);
    }

    private static string NormalizeSqlServerDataType(string dataTypeName)
    {
        if (string.IsNullOrEmpty(dataTypeName))
            return "unknown";

        return dataTypeName.ToLowerInvariant().Trim() switch
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

            // Date/Time types (without timezone)
            "datetime" or "datetime2" or "smalldatetime" => "datetime",
            
            // Date/Time types (with timezone)
            "datetimeoffset" => "datetimeoffset",
            
            // Date only
            "date" => "date",
            
            // Time only
            "time" => "time",

            // UUID/GUID types
            "uniqueidentifier" => "guid",

            // Binary types
            "varbinary" or "binary" or "image" or "timestamp" => "binary",
            
            // Fallback for unknown types
            _ => "unknown"
        };
    }
} 