using System;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

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
            IsCollection = column.LanguageDataType.EndsWith("[]"),
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
            IsNullable = parameter.Direction != StoredProcedureParameterDirection.Out, // Input parameters can be nullable, output parameters typically aren't
            IsCollection = parameter.LanguageDataType.EndsWith("[]"),
            Stereotypes = [],
            GenericTypeParameters = []
        };
    }

    /// <summary>
    /// Maps a stored procedure parameter to TypeReference when a UserDefinedTable DataContract is available
    /// </summary>
    public static TypeReferencePersistable MapStoredProcedureParameterTypeToTypeReference(StoredProcedureParameterSchema parameter, string? userDefinedTableDataContractId)
    {
        // If we have a UserDefinedTable DataContract, reference it
        if (parameter.UserDefinedTableType != null && !string.IsNullOrEmpty(userDefinedTableDataContractId))
        {
            return new TypeReferencePersistable
            {
                Id = Guid.NewGuid().ToString(),
                TypeId = userDefinedTableDataContractId,
                IsNullable = parameter.Direction != StoredProcedureParameterDirection.Out,
                IsCollection = true, // UserDefinedTable parameters are collections
                Stereotypes = [],
                GenericTypeParameters = []
            };
        }

        // Fall back to original behavior for non-UDT parameters
        return MapStoredProcedureParameterTypeToTypeReference(parameter);
    }

    public static TypeReferencePersistable MapResultSetColumnTypeToTypeReference(ResultSetColumnSchema column)
    {
        return new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = GetTypeId(column.LanguageDataType),
            IsNullable = column.IsNullable,
            IsCollection = column.LanguageDataType.EndsWith("[]"),
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
            "string[]" => Constants.TypeDefinitions.CommonTypes.String,
            "short[]" => Constants.TypeDefinitions.CommonTypes.Short,
            "int[]" => Constants.TypeDefinitions.CommonTypes.Int,
            "long[]" => Constants.TypeDefinitions.CommonTypes.Long,
            "guid[]" => Constants.TypeDefinitions.CommonTypes.Guid,
            "byte[][]" => Constants.TypeDefinitions.CommonTypes.Binary,
            _ => Constants.TypeDefinitions.CommonTypes.Object
        };
    }
}
