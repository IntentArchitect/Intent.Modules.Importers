using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating ElementPersistable test objects
/// </summary>
internal static class Elements
{
    public static ElementPersistable SimpleUsersTable(string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = "User",
        SpecializationType = ClassModel.SpecializationType,
        SpecializationTypeId = ClassModel.SpecializationTypeId,
        ExternalReference = ModelNamingUtilities.GetTableExternalReference("dbo", "Users"),
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.Table("dbo", "Users")
        },
        ChildElements = new List<ElementPersistable>
        {
            Attribute("Id", "int", ModelNamingUtilities.GetColumnExternalReference("dbo", "Users", "Id"), isPrimaryKey: true),
            Attribute("Name", "string", ModelNamingUtilities.GetColumnExternalReference("dbo", "Users", "Name"))
        }
    };

    public static ElementPersistable SimpleCustomersTable(string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = "Customer",
        SpecializationType = ClassModel.SpecializationType,
        SpecializationTypeId = ClassModel.SpecializationTypeId,
        ExternalReference = ModelNamingUtilities.GetTableExternalReference("dbo", "Customers"),
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.Table("dbo", "Customers")
        },
        ChildElements = new List<ElementPersistable>
        {
            Attribute("Id", "int", ModelNamingUtilities.GetColumnExternalReference("dbo", "Customers", "Id"), isPrimaryKey: true),
            Attribute("Email", "string", ModelNamingUtilities.GetColumnExternalReference("dbo", "Customers", "Email"))
        }
    };

    public static ElementPersistable OrdersTableWithCustomerFk(string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = "Order",
        SpecializationType = ClassModel.SpecializationType,
        SpecializationTypeId = ClassModel.SpecializationTypeId,
        ExternalReference = ModelNamingUtilities.GetTableExternalReference("dbo", "Orders"),
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.Table("dbo", "Orders")
        },
        ChildElements = new List<ElementPersistable>
        {
            Attribute("Id", "int", ModelNamingUtilities.GetColumnExternalReference("dbo", "Orders", "Id"), isPrimaryKey: true),
            Attribute("CustomerId", "int", ModelNamingUtilities.GetColumnExternalReference("dbo", "Orders", "CustomerId"))
        }
    };

    public static ElementPersistable Attribute(
        string name, 
        string typeId, 
        string? externalReference = null,
        bool isPrimaryKey = false,
        bool isNullable = false,
        string? foreignKeyAssociationTargetEndId = null)
    {
        var attr = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            SpecializationType = AttributeModel.SpecializationType,
            SpecializationTypeId = AttributeModel.SpecializationTypeId,
            ExternalReference = externalReference,
            TypeReference = new TypeReferencePersistable
            {
                TypeId = typeId,
                IsNullable = isNullable,
                IsCollection = false
            },
            Stereotypes = new List<StereotypePersistable>()
        };

        if (isPrimaryKey)
        {
            attr.Stereotypes.Add(Stereotypes.PrimaryKey());
        }

        if (!string.IsNullOrEmpty(foreignKeyAssociationTargetEndId))
        {
            attr.Stereotypes.Add(Stereotypes.ForeignKeyAttribute(foreignKeyAssociationTargetEndId));
        }

        return attr;
    }

    public static ElementPersistable Repository(string name, string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = name,
        SpecializationType = "Repository",
        ExternalReference = $"repository:{name}",
        ChildElements = new List<ElementPersistable>()
    };

    public static ElementPersistable StoredProcedureOperation(
        string name, 
        string? externalReference = null,
        List<ElementPersistable>? parameters = null) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = name,
        SpecializationType = OperationModel.SpecializationType,
        SpecializationTypeId = OperationModel.SpecializationTypeId,
        ExternalReference = externalReference,
        ChildElements = parameters ?? new List<ElementPersistable>()
    };

    public static ElementPersistable DataContract(
        string name, 
        string? externalReference = null,
        List<ElementPersistable>? properties = null) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = name,
        SpecializationType = "Data Contract",
        ExternalReference = externalReference,
        ChildElements = properties ?? new List<ElementPersistable>()
    };

    /// <summary>
    /// Table A element for three-table scenario testing.
    /// Has FK attribute pointing to Table B.
    /// </summary>
    public static ElementPersistable TableA(string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = "TableA",
        SpecializationType = ClassModel.SpecializationType,
        SpecializationTypeId = ClassModel.SpecializationTypeId,
        ExternalReference = ModelNamingUtilities.GetTableExternalReference("dbo", "TableA"),
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.Table("dbo", "TableA")
        },
        ChildElements = new List<ElementPersistable>
        {
            Attribute("Id", "int", ModelNamingUtilities.GetColumnExternalReference("dbo", "TableA", "Id"), isPrimaryKey: true),
            Attribute("Name", "string", ModelNamingUtilities.GetColumnExternalReference("dbo", "TableA", "Name")),
            AttributeWithForeignKey("TableBId", "int", ModelNamingUtilities.GetColumnExternalReference("dbo", "TableA", "TableBId"))
        }
    };

    /// <summary>
    /// Table B element for three-table scenario testing.
    /// Has FK attribute pointing to Table C (will be outdated after import).
    /// </summary>
    public static ElementPersistable TableB(string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = "TableB",
        SpecializationType = ClassModel.SpecializationType,
        SpecializationTypeId = ClassModel.SpecializationTypeId,
        ExternalReference = ModelNamingUtilities.GetTableExternalReference("dbo", "TableB"),
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.Table("dbo", "TableB")
        },
        ChildElements = new List<ElementPersistable>
        {
            Attribute("Id", "int", ModelNamingUtilities.GetColumnExternalReference("dbo", "TableB", "Id"), isPrimaryKey: true),
            Attribute("Description", "string", ModelNamingUtilities.GetColumnExternalReference("dbo", "TableB", "Description")),
            AttributeWithForeignKey("TableCId", "int", ModelNamingUtilities.GetColumnExternalReference("dbo", "TableB", "TableCId"))
        }
    };

    /// <summary>
    /// Table C element for three-table scenario testing.
    /// </summary>
    public static ElementPersistable TableC(string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = "TableC",
        SpecializationType = ClassModel.SpecializationType,
        SpecializationTypeId = ClassModel.SpecializationTypeId,
        ExternalReference = ModelNamingUtilities.GetTableExternalReference("dbo", "TableC"),
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.Table("dbo", "TableC")
        },
        ChildElements = new List<ElementPersistable>
        {
            Attribute("Id", "int", ModelNamingUtilities.GetColumnExternalReference("dbo", "TableC", "Id"), isPrimaryKey: true),
            Attribute("Category", "string", ModelNamingUtilities.GetColumnExternalReference("dbo", "TableC", "Category"))
        }
    };

    /// <summary>
    /// Creates an attribute with Foreign Key stereotype
    /// </summary>
    public static ElementPersistable AttributeWithForeignKey(
        string name, 
        string typeId, 
        string? externalReference = null,
        bool isNullable = false) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = name,
        SpecializationType = AttributeModel.SpecializationType,
        SpecializationTypeId = AttributeModel.SpecializationTypeId,
        ExternalReference = externalReference,
        TypeReference = new TypeReferencePersistable
        {
            TypeId = typeId,
            IsNullable = isNullable,
            IsCollection = false
        },
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.ForeignKey(name)
        }
    };
}
