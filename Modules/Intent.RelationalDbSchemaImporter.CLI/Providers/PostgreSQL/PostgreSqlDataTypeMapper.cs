using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

/// <summary>
/// PostgreSQL-specific data type mapper with custom type handling
/// </summary>
internal class PostgreSqlDataTypeMapper : DefaultDataTypeMapper
{
    /// <summary>
    /// Override to handle PostgreSQL-specific data types
    /// </summary>
    /// <remarks>
    /// SERIAL / BIGSERIAL aren't real datatypes - syntactic sugar for autoincrement columns - is actually INT4 / INT8. 
    /// </remarks>
    public override string GetNormalizedDataTypeString(DataType? dataType, string dbDataType)
    {
        // Handle PostgreSQL-specific data types
        switch (dbDataType.ToLowerInvariant())
        {
            case "uuid":
                return "string";
            case "bytea":
                return "binary";
            case "jsonb":
            case "json":
                return "string";
            case "_text":
            case "text[]":
                return "string[]";
            case "trigger":
                return "object";
            case "record":
                return "object";
            default:
                return base.GetNormalizedDataTypeString(dataType, dbDataType);
        }
    }
} 