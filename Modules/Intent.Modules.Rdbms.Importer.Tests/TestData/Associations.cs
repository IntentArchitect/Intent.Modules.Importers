using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating AssociationPersistable test objects
/// </summary>
internal static class Associations
{
    public static AssociationPersistable OrdersToCustomersFk(
        string ordersClassId, 
        string customersClassId,
        string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        ExternalReference = ModelNamingUtilities.GetForeignKeyExternalReference("dbo", "Orders", "FK_Orders_Customers"),
        SourceEnd = new AssociationEndPersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeReference = new TypeReferencePersistable
            {
                TypeId = ordersClassId,
                IsNavigable = false,
                IsNullable = false,
                IsCollection = true
            }
        },
        TargetEnd = new AssociationEndPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Customer",
            TypeReference = new TypeReferencePersistable
            {
                TypeId = customersClassId,
                IsNavigable = true,
                IsNullable = false,
                IsCollection = false
            }
        },
        Stereotypes = [Stereotypes.ForeignKey("CustomerId")]
    };

    public static AssociationPersistable Simple(
        string sourceClassId,
        string targetClassId,
        string externalReference,
        string sourceEndName = "Source",
        string targetEndName = "Target",
        bool sourceIsCollection = true,
        bool targetIsCollection = false) => new()
    {
        Id = Guid.NewGuid().ToString(),
        ExternalReference = externalReference,
        SourceEnd = new AssociationEndPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = sourceEndName,
            TypeReference = new TypeReferencePersistable
            {
                TypeId = sourceClassId,
                IsNavigable = false,
                IsNullable = false,
                IsCollection = sourceIsCollection
            }
        },
        TargetEnd = new AssociationEndPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = targetEndName,
            TypeReference = new TypeReferencePersistable
            {
                TypeId = targetClassId,
                IsNavigable = true,
                IsNullable = false,
                IsCollection = targetIsCollection
            }
        },
        Stereotypes = []
    };

    /// <summary>
    /// Association from Table A to Table B via FK
    /// </summary>
    public static AssociationPersistable TableAToTableB(
        string tableAClassId, 
        string tableBClassId,
        string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        ExternalReference = ModelNamingUtilities.GetForeignKeyExternalReference("dbo", "TableA", "FK_TableA_TableB"),
        SourceEnd = new AssociationEndPersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeReference = new TypeReferencePersistable
            {
                TypeId = tableAClassId,
                IsNavigable = false,
                IsNullable = false,
                IsCollection = true
            }
        },
        TargetEnd = new AssociationEndPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TableB",
            TypeReference = new TypeReferencePersistable
            {
                TypeId = tableBClassId,
                IsNavigable = true,
                IsNullable = false,
                IsCollection = false
            }
        },
        Stereotypes = [Stereotypes.ForeignKey("TableBId")]
    };

    /// <summary>
    /// Association from Table B to Table C via FK (this should be removed during import)
    /// </summary>
    public static AssociationPersistable TableBToTableC(
        string tableBClassId, 
        string tableCClassId,
        string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        ExternalReference = ModelNamingUtilities.GetForeignKeyExternalReference("dbo", "TableB", "FK_TableB_TableC"),
        SourceEnd = new AssociationEndPersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeReference = new TypeReferencePersistable
            {
                TypeId = tableBClassId,
                IsNavigable = false,
                IsNullable = false,
                IsCollection = true
            }
        },
        TargetEnd = new AssociationEndPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TableC",
            TypeReference = new TypeReferencePersistable
            {
                TypeId = tableCClassId,
                IsNavigable = true,
                IsNullable = false,
                IsCollection = false
            }
        },
        Stereotypes = [Stereotypes.ForeignKey("TableCId")]
    };
}
