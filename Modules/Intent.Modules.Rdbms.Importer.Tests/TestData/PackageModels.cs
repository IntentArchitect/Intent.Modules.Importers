using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.Modules.Rdbms.Importer.Tests.TestData;

/// <summary>
/// Object Mother for creating common PackageModelPersistable test scenarios
/// </summary>
internal static class PackageModels
{
    public static PackageModelPersistable Empty() => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "TestPackage",
        Classes = new List<ElementPersistable>(),
        Associations = new List<AssociationPersistable>()
    };

    public static PackageModelPersistable WithSimpleTable()
    {
        var package = Empty();
        package.Classes.Add(Elements.SimpleUsersTable());
        return package;
    }

    public static PackageModelPersistable WithTableAndForeignKey()
    {
        var package = Empty();
        var customers = Elements.SimpleCustomersTable();
        var orders = Elements.OrdersTableWithCustomerFk();
        
        package.Classes.Add(customers);
        package.Classes.Add(orders);
        package.Associations.Add(Associations.OrdersToCustomersFk(orders.Id, customers.Id));
        
        return package;
    }

    public static PackageModelPersistable WithRepository()
    {
        var package = Empty();
        package.Classes.Add(Elements.Repository("CustomerRepository"));
        return package;
    }

    public static PackageModelPersistable WithCustomerTable()
    {
        var package = Empty();
        package.Classes.Add(Elements.SimpleCustomersTable());
        return package;
    }

    public static PackageModelPersistable WithCustomerAndOrderTables()
    {
        var package = Empty();
        var customers = Elements.SimpleCustomersTable();
        var orders = Elements.OrdersTableWithCustomerFk();
        
        package.Classes.Add(customers);
        package.Classes.Add(orders);
        package.Associations.Add(Associations.OrdersToCustomersFk(orders.Id, customers.Id));
        
        return package;
    }

    public static PackageModelPersistable WithExistingCustomer()
    {
        var package = Empty();
        package.Classes.Add(Elements.SimpleCustomersTable());
        return package;
    }

    public static PackageModelPersistable WithUserHavingCustomEnumType()
    {
        var package = Empty();
        var userClass = Elements.SimpleUsersTable();
        
        // Add a Status attribute with a custom enum type instead of the default int
        var statusAttribute = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Status",
            SpecializationType = "Attribute",
            SpecializationTypeId = "c8e1ed70-26bb-4e10-9d2a-30c35bb26471",
            ExternalReference = "[dbo].[users].[status]",
            TypeReference = new TypeReferencePersistable
            {
                TypeId = "custom-enum-id", // Custom enum type ID
                IsNullable = false
            }
        };
        
        userClass.ChildElements.Add(statusAttribute);
        package.Classes.Add(userClass);
        
        return package;
    }
}
