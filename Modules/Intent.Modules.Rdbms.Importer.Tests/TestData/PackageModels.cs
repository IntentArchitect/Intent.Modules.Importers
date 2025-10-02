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
}
