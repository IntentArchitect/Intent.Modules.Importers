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
        var association = Associations.OrdersToCustomersFk(orders.Id, customers.Id);
        package.Associations.Add(association);
        
        // Add FK stereotype to the CustomerId attribute linking to the association's target end
        var customerIdAttribute = orders.ChildElements.FirstOrDefault(a => a.Name == "CustomerId");
        if (customerIdAttribute != null)
        {
            customerIdAttribute.Stereotypes.Add(Stereotypes.ForeignKeyAttribute(association.TargetEnd.Id));
        }
        
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

    /// <summary>
    /// Package with three tables (A, B, C) where A→B and B→C associations exist.
    /// This represents the existing state before inclusive import where B→C FK was removed from DB.
    /// </summary>
    public static PackageModelPersistable WithTableABCAndBothAssociations()
    {
        var package = Empty();
        var tableA = Elements.TableA();
        var tableB = Elements.TableB();
        var tableC = Elements.TableC();
        
        package.Classes.Add(tableA);
        package.Classes.Add(tableB);
        package.Classes.Add(tableC);
        
        // Both associations exist in the package (representing outdated state)
        package.Associations.Add(Associations.TableAToTableB(tableA.Id, tableB.Id));
        package.Associations.Add(Associations.TableBToTableC(tableB.Id, tableC.Id));
        
        return package;
    }
}
