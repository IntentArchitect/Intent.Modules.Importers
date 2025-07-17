using System;
using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Schema;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.Modules.SqlServerImporter.Tasks.Mappers;

public class TypeReferenceMapper
{
    public TypeReferencePersistable MapColumnTypeToTypeReference(ColumnSchema column)
    {
        return new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = GetTypeId(column.DataType),
            IsNullable = column.IsNullable,
            IsCollection = false,
            Stereotypes = new List<StereotypePersistable>(),
            GenericTypeParameters = new List<TypeReferencePersistable>()
        };
    }

    public TypeReferencePersistable MapStoredProcedureParameterTypeToTypeReference(StoredProcedureParameterSchema parameter)
    {
        return new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = GetTypeId(parameter.DataType),
            IsNullable = !parameter.IsOutputParameter, // Input parameters can be nullable, output parameters typically aren't
            IsCollection = parameter.DataType.ToLower() == "user-defined-table-type",
            Stereotypes = new List<StereotypePersistable>(),
            GenericTypeParameters = new List<TypeReferencePersistable>()
        };
    }

    public TypeReferencePersistable MapResultSetColumnTypeToTypeReference(ResultSetColumnSchema column)
    {
        return new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = GetTypeId(column.DataType),
            IsNullable = column.IsNullable,
            IsCollection = false,
            Stereotypes = new List<StereotypePersistable>(),
            GenericTypeParameters = new List<TypeReferencePersistable>()
        };
    }

    /// <summary>
    /// Creates a type reference for stored procedure return types
    /// </summary>
    public TypeReferencePersistable CreateStoredProcedureReturnTypeReference(string? dataContractId, bool hasMultipleRows = true)
    {
        return new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = dataContractId,
            IsNullable = false,
            IsCollection = hasMultipleRows,
            Stereotypes = new List<StereotypePersistable>(),
            GenericTypeParameters = new List<TypeReferencePersistable>()
        };
    }

    /// <summary>
    /// Creates a type reference for association ends
    /// </summary>
    public TypeReferencePersistable CreateAssociationTypeReference(string targetClassId, bool isNullable, bool isCollection, bool isNavigable = true)
    {
        return new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = targetClassId,
            IsNullable = isNullable,
            IsCollection = isCollection,
            IsNavigable = isNavigable,
            Stereotypes = new List<StereotypePersistable>(),
            GenericTypeParameters = new List<TypeReferencePersistable>()
        };
    }

    private string? GetTypeId(string dataType)
    {
        // Convert database-agnostic data type string to Intent type ID
        return dataType.ToLower() switch
        {
            "varchar" or "nvarchar" or "text" or "ntext" or "char" or "nchar" or "sysname" or "xml" => Constants.TypeDefinitions.CommonTypes.String,
            "int" => Constants.TypeDefinitions.CommonTypes.Int,
            "bigint" => Constants.TypeDefinitions.CommonTypes.Long,
            "smallint" => Constants.TypeDefinitions.CommonTypes.Short,
            "tinyint" => Constants.TypeDefinitions.CommonTypes.Byte,
            "decimal" or "numeric" or "money" or "smallmoney" or "float" or "real" => Constants.TypeDefinitions.CommonTypes.Decimal,
            "bit" => Constants.TypeDefinitions.CommonTypes.Bool,
            "datetime" or "datetime2" or "smalldatetime" => Constants.TypeDefinitions.CommonTypes.Datetime,
            "date" => Constants.TypeDefinitions.CommonTypes.Date,
            "time" => Constants.TypeDefinitions.CommonTypes.TimeSpan,
            "uniqueidentifier" => Constants.TypeDefinitions.CommonTypes.Guid,
            "varbinary" or "binary" or "image" or "timestamp" => Constants.TypeDefinitions.CommonTypes.Binary,
            "datetimeoffset" => Constants.TypeDefinitions.CommonTypes.DatetimeOffset,
            _ => null
        };
    }
}
