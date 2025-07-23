using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

internal class PostgreSqlDataTypeMapper : DefaultDataTypeMapper
{
    public override string GetNormalizedDataTypeString(DataType? dataType, string dbDataType)
    {
        // NOTES:
        // * SERIAL / BIGSERIAL aren't real datatypes - syntactic sugar for autoincrement columns - is actually INT4 / INT8.
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