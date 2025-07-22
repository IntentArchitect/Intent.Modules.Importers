using System;
using DatabaseSchemaReader.DataSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class DataTypeMapperBase
{
    public abstract string GetDataTypeString(string? dataTypeName);
    public abstract string GetNormalizedDataTypeString(DataType? dataType, string dbDataType);
}

/// <summary>
/// Base implementation for database data type mapping
/// </summary>
internal class DefaultDataTypeMapper : DataTypeMapperBase
{
    /// <summary>
    /// Gets the base SQL data type name without length/precision information
    /// </summary>
    /// <remarks>
    /// PostgresSQL: SERIAL / BIGSERIAL aren't real datatypes - syntactic sugar for autoincrement columns - is actually INT4 / INT8. 
    /// </remarks>
    public override string GetDataTypeString(string? dataTypeName)
    {
        if (string.IsNullOrEmpty(dataTypeName))
            return "unknown";

        // Strip length/precision information (e.g., "nvarchar(255)" -> "nvarchar", "decimal(18,2)" -> "decimal")
        var baseTypeName = dataTypeName;
        var parenIndex = baseTypeName.IndexOf('(');
        if (parenIndex > 0)
        {
            baseTypeName = baseTypeName.Substring(0, parenIndex);
        }

        return baseTypeName.Trim().ToLowerInvariant();
    }

    /// <remarks>
    /// If you get an exception thrown here, you will need to perform an override on this method in the respective database provider.
    /// Then you will need to perform the "dbDataType" checks first to determine the appropriate C# type.
    /// After that call this base method.
    /// </remarks>
    public override string GetNormalizedDataTypeString(DataType? dataType, string dbDataType)
    {
        return dataType?.NetDataTypeCSharpName?.ToLowerInvariant() ?? throw new InvalidOperationException($"Unable to extract normalized data type for database data type '{dbDataType}'");
    }
} 