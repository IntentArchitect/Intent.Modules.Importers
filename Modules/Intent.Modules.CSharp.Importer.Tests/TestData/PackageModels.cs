using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Persistence;

namespace Intent.Modules.CSharp.Importer.Tests.TestData;

/// <summary>
/// Factory methods for creating test packages wrapped in TestPackageWrapper
/// to stub GetReferencedPackages() for test isolation.
/// </summary>
internal static class PackageModels
{
    public static IPackageModelPersistable Empty() => new TestPackageWrapper(new PackageModelPersistable
    {
        Id = Guid.NewGuid().ToString(),
        Name = "TestPackage",
        Classes = new List<ElementPersistable>(),
        Associations = new List<AssociationPersistable>()
    });

    public static IPackageModelPersistable WithCustomerClass()
    {
        var package = new PackageModelPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestPackage",
            Classes = new List<ElementPersistable>(),
            Associations = new List<AssociationPersistable>()
        };
        
        package.Classes.Add(new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Customer",
            SpecializationType = "Class",
            SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10",
            ExternalReference = "TestApp.Domain.Customer",
            ChildElements = new List<ElementPersistable>
            {
                new ElementPersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Id",
                    SpecializationType = "Attribute",
                    SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf"
                },
                new ElementPersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Email",
                    SpecializationType = "Attribute",
                    SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf"
                }
            }
        });
        return new TestPackageWrapper(package);
    }

    public static IPackageModelPersistable WithCustomerAndOrderClasses()
    {
        var package = new PackageModelPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestPackage",
            Classes = new List<ElementPersistable>(),
            Associations = new List<AssociationPersistable>()
        };
        
        var customer = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Customer",
            SpecializationType = "Class",
            SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10",
            ExternalReference = "TestApp.Domain.Customer",
            ChildElements = new List<ElementPersistable>
            {
                new ElementPersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "CustomerId",
                    SpecializationType = "Attribute",
                    SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf"
                }
            }
        };

        var order = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Order",
            SpecializationType = "Class",
            SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10",
            ExternalReference = "TestApp.Domain.Order",
            ChildElements = new List<ElementPersistable>
            {
                new ElementPersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "OrderId",
                    SpecializationType = "Attribute",
                    SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf"
                }
            }
        };

        package.Classes.Add(customer);
        package.Classes.Add(order);
        
        return new TestPackageWrapper(package);
    }

    public static IPackageModelPersistable WithFolder(string packageName, IEnumerable<IPackageModelPersistable> referencedPackages = null)
    {
        var package = new PackageModelPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = packageName,
            Classes = new List<ElementPersistable>(),
            Associations = new List<AssociationPersistable>()
        };

        var dboFolder = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Folder",
            SpecializationType = "Folder",
            SpecializationTypeId = "4d95d53a-8855-4f35-aa82-e312643f5c5f",
            ExternalReference = "dbo.folder",
            PackageId = package.Id
        };

        package.ChildElements.Add(dboFolder);

        var wrapper = new TestPackageWrapper(package);

        if(referencedPackages is not null)
        {
            wrapper.SetReferencedPackages(referencedPackages);
        }

        return wrapper;
    }
}
