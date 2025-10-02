using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;

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
        ExternalReference = "table:dbo.Users",
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.Table("dbo", "Users")
        },
        ChildElements = new List<ElementPersistable>
        {
            Attribute("Id", "int", "column:dbo.Users.Id", isPrimaryKey: true),
            Attribute("Name", "string", "column:dbo.Users.Name")
        }
    };

    public static ElementPersistable SimpleCustomersTable(string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = "Customer",
        SpecializationType = ClassModel.SpecializationType,
        SpecializationTypeId = ClassModel.SpecializationTypeId,
        ExternalReference = "table:dbo.Customers",
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.Table("dbo", "Customers")
        },
        ChildElements = new List<ElementPersistable>
        {
            Attribute("Id", "int", "column:dbo.Customers.Id", isPrimaryKey: true),
            Attribute("Email", "string", "column:dbo.Customers.Email")
        }
    };

    public static ElementPersistable OrdersTableWithCustomerFk(string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = "Order",
        SpecializationType = ClassModel.SpecializationType,
        SpecializationTypeId = ClassModel.SpecializationTypeId,
        ExternalReference = "table:dbo.Orders",
        Stereotypes = new List<StereotypePersistable>
        {
            Stereotypes.Table("dbo", "Orders")
        },
        ChildElements = new List<ElementPersistable>
        {
            Attribute("Id", "int", "column:dbo.Orders.Id", isPrimaryKey: true),
            Attribute("CustomerId", "int", "column:dbo.Orders.CustomerId")
        }
    };

    public static ElementPersistable Attribute(
        string name, 
        string typeId, 
        string? externalReference = null,
        bool isPrimaryKey = false,
        bool isNullable = false)
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
}
