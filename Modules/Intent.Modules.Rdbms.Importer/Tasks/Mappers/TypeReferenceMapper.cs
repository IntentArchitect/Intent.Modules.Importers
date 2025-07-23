using System;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

internal static class TypeReferenceMapper
{
    public static TypeReferencePersistable MapColumnTypeToTypeReference(ColumnSchema column)
    {
        return new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = GetTypeId(column.LanguageDataType),
            IsNullable = column.IsNullable,
            IsCollection = false,
            Stereotypes = [],
            GenericTypeParameters = []
        };
    }

    public static TypeReferencePersistable MapStoredProcedureParameterTypeToTypeReference(StoredProcedureParameterSchema parameter)
    {
        return new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = GetTypeId(parameter.LanguageDataType),
            IsNullable = !parameter.IsOutputParameter, // Input parameters can be nullable, output parameters typically aren't
            IsCollection = parameter.DbDataType.ToLower() == "user-defined-table-type",
            Stereotypes = [],
            GenericTypeParameters = []
        };
    }

    public static TypeReferencePersistable MapResultSetColumnTypeToTypeReference(ResultSetColumnSchema column)
    {
        return new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = GetTypeId(column.LanguageDataType),
            IsNullable = column.IsNullable,
            IsCollection = false,
            Stereotypes = [],
            GenericTypeParameters = []
        };
    }

    private static string? GetTypeId(string normalizedDataType)
    {
        // Map fundamental types to Intent common types - simple 1:1 mapping
        return normalizedDataType switch
        {
            "string" => Constants.TypeDefinitions.CommonTypes.String,
            "int" => Constants.TypeDefinitions.CommonTypes.Int,
            "long" => Constants.TypeDefinitions.CommonTypes.Long,
            "short" => Constants.TypeDefinitions.CommonTypes.Short,
            "byte" => Constants.TypeDefinitions.CommonTypes.Byte,
            "decimal" => Constants.TypeDefinitions.CommonTypes.Decimal,
            "bool" => Constants.TypeDefinitions.CommonTypes.Bool,
            "datetime" => Constants.TypeDefinitions.CommonTypes.Datetime,
            "datetimeoffset" => Constants.TypeDefinitions.CommonTypes.DatetimeOffset,
            "date" => Constants.TypeDefinitions.CommonTypes.Date,
            "time" => Constants.TypeDefinitions.CommonTypes.TimeSpan,
            "guid" => Constants.TypeDefinitions.CommonTypes.Guid,
            "binary" => Constants.TypeDefinitions.CommonTypes.Binary,
            "byte[]" => Constants.TypeDefinitions.CommonTypes.Binary,
            _ => null
        };
    }
}
