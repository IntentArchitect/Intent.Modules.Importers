using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

internal class PostgreSqlDataTypeMapper : DefaultDataTypeMapper
{
    public override string GetDbDataTypeString(string? dataTypeName)
    {
        return base.GetDbDataTypeString(dataTypeName).Replace("\"", string.Empty);
    }

    public override string GetLanguageDataTypeString(DataType? dataType, string dbDataType)
    {
        var normalized = dbDataType.ToLowerInvariant().Replace("\"", string.Empty);
        // https://www.postgresql.org/docs/current/datatype.html
        return normalized switch
        {
            "smallint" or "int2" => "short",
            "integer" or "int" or "int4" => "int",
            "bigint" or "int8" => "long",
            "serial" or "serial4" => "int",
            "bigserial" or "serial8" => "long",
            "real" or "float4" => "float",
            "double precision" or "float8" => "double",
            "numeric" or "decimal" or "money" => "decimal",
            "boolean" or "bool" => "bool",
            "char" or "character" or "character varying" or "varchar" or "text" or "name" => "string",
            "uuid" => "guid",
            "date" or "timestamp" or "timestamp without time zone" or "timestamp with time zone" or "timestamptz" => "datetime",
            "time with time zone" or "timetz" => "datetimeoffset",
            "time" or "interval" => "time",
            "bytea" => "binary",
            "json" or "jsonb" or "xml" => "string",
            "cidr" or "macaddr" => "string",
            "point" or "line" or "lseg" or "box" or "path" or "polygon" or "circle" => "string", // or a custom geometry type
            
            // Arrays (e.g., int[], text[], etc.)
            "int2[]" or "smallint[]" => "short[]",
            "int4[]" or "integer[]" => "int[]",
            "int8[]" or "bigint[]" => "long[]",
            "text[]" or "varchar[]" or "character varying[]" => "string[]",
            "uuid[]" => "guid[]",
            "bytea[]" => "byte[][]",
            "_int4" or "_integer" => "int[]",
            "_int8" or "_bigint" => "long[]",
            "_text" or "_varchar" or "_character varying" => "string[]",
            "_uuid" => "guid[]",
            "_bytea" => "byte[][]",
            _ => base.GetLanguageDataTypeString(dataType, dbDataType)
        };
    }
}