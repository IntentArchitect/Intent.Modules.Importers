using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Json.Importer.Tests.TestData;

/// <summary>
/// Object Mother factory for creating PackageModelPersistable instances.
/// </summary>
public static class PackageModels
{
    public static PackageModelPersistable EmptyDomainPackage() => CreatePackage(
        name: "TestDomain",
        specializationType: "Domain Package",
        specializationTypeId: "1a824508-4623-45d9-accc-f572091ade5a");

    public static PackageModelPersistable EmptyServicesPackage() => CreatePackage(
        name: "TestServices",
        specializationType: "Services Package",
        specializationTypeId: "df45eaf6-9202-4c03-90fb-dad1e2a45e4f");

    public static PackageModelPersistable WithDomainTypes()
    {
        var package = new PackageModelPersistable
        {
            Id = "test-package-id",
            Name = "TestDomain",
            SpecializationType = "Domain Package",
            SpecializationTypeId = "1a824508-4623-45d9-accc-f572091ade5a",
            Classes = new List<ElementPersistable>(),
            Associations = new List<AssociationPersistable>()
        };

        // Add type definitions as both Classes and to the types list
        var types = new List<ElementPersistable>
        {
            CreateTypeDefinition("string", package.Id, package.Name),
            CreateTypeDefinition("bool", package.Id, package.Name),
            CreateTypeDefinition("decimal", package.Id, package.Name),
            CreateTypeDefinition("guid", package.Id, package.Name),
            CreateTypeDefinition("datetime", package.Id, package.Name),
            CreateTypeDefinition("object", package.Id, package.Name)
        };

        foreach (var type in types)
        {
            package.Classes.Add(type);
        }

        return package;
    }

    public static PackageModelPersistable WithServicesTypes()
    {
        var package = new PackageModelPersistable
        {
            Id = "test-services-package-id",
            Name = "TestServices",
            SpecializationType = "Services Package",
            SpecializationTypeId = "df45eaf6-9202-4c03-90fb-dad1e2a45e4f",
            Classes = new List<ElementPersistable>(),
            Associations = new List<AssociationPersistable>()
        };

        // Add type definitions as both Classes and to the types list
        var types = new List<ElementPersistable>
        {
            CreateTypeDefinition("string", package.Id, package.Name),
            CreateTypeDefinition("bool", package.Id, package.Name),
            CreateTypeDefinition("decimal", package.Id, package.Name),
            CreateTypeDefinition("guid", package.Id, package.Name),
            CreateTypeDefinition("datetime", package.Id, package.Name),
            CreateTypeDefinition("object", package.Id, package.Name)
        };

        foreach (var type in types)
        {
            package.Classes.Add(type);
        }

        return package;
    }

    private static PackageModelPersistable CreatePackage(
        string name,
        string specializationType,
        string specializationTypeId)
    {
        return new PackageModelPersistable
        {
            Id = $"test-{name.ToLower()}-id",
            Name = name,
            SpecializationType = specializationType,
            SpecializationTypeId = specializationTypeId,
            Classes = new List<ElementPersistable>(),
            Associations = new List<AssociationPersistable>()
        };
    }

    /// <summary>
    /// Creates a domain package with an existing Customer class that matches the simple customer JSON.
    /// Used for testing merge/re-import scenarios.
    /// </summary>
    public static PackageModelPersistable WithExistingCustomer()
    {
        var package = WithDomainTypes();
        
        var customerClass = CreateClass(
            name: "SimpleCustomer",
            packageId: package.Id,
            externalReference: "simple-customer.json",
            specializationType: "Class",
            specializationTypeId: "04e12b51-ed12-42a3-9667-a6aa81bb6d10");
        
        // Add attributes that match simple customer JSON
        customerClass.ChildElements.Add(CreateAttribute(
            name: "Id",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Id"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            name: "Name",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Name"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            name: "Email",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Email"));
        
        package.Classes.Add(customerClass);
        return package;
    }

    /// <summary>
    /// Creates a domain package with existing Customer and Invoice classes.
    /// Used for testing merge scenarios with multiple existing elements.
    /// </summary>
    public static PackageModelPersistable WithCustomerAndInvoice()
    {
        var package = WithDomainTypes();
        
        var customerClass = CreateClass(
            name: "SimpleCustomer",
            packageId: package.Id,
            externalReference: "simple-customer.json",
            specializationType: "Class",
            specializationTypeId: "04e12b51-ed12-42a3-9667-a6aa81bb6d10");
        
        customerClass.ChildElements.Add(CreateAttribute(
            name: "Id",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Id"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            name: "Name",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Name"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            name: "Email",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Email"));
        
        var invoiceClass = CreateClass(
            name: "Invoice",
            packageId: package.Id,
            externalReference: "invoice.json",
            specializationType: "Class",
            specializationTypeId: "04e12b51-ed12-42a3-9667-a6aa81bb6d10");
        
        invoiceClass.ChildElements.Add(CreateAttribute(
            name: "Id",
            typeId: GetTypeId(package, "string"),
            parentId: invoiceClass.Id,
            externalReference: "invoice.json.Id"));
        
        invoiceClass.ChildElements.Add(CreateAttribute(
            name: "InvoiceNumber",
            typeId: GetTypeId(package, "string"),
            parentId: invoiceClass.Id,
            externalReference: "invoice.json.InvoiceNumber"));
        
        invoiceClass.ChildElements.Add(CreateAttribute(
            name: "Amount",
            typeId: GetTypeId(package, "decimal"),
            parentId: invoiceClass.Id,
            externalReference: "invoice.json.Amount"));
        
        package.Classes.Add(customerClass);
        package.Classes.Add(invoiceClass);
        return package;
    }

    /// <summary>
    /// Creates a services package with an existing Account DTO.
    /// Used for testing merge/re-import scenarios.
    /// </summary>
    public static PackageModelPersistable WithExistingAccountDto()
    {
        var package = WithServicesTypes();
        
        var accountDto = CreateClass(
            name: "Account",
            packageId: package.Id,
            externalReference: "account.json",
            specializationType: "DTO",
            specializationTypeId: "c2188e49-2989-43f8-b1a4-3263d56af4f7");
        
        accountDto.ChildElements.Add(CreateAttribute(
            name: "AccountId",
            typeId: GetTypeId(package, "guid"),
            parentId: accountDto.Id,
            externalReference: "account.json.accountId"));
        
        accountDto.ChildElements.Add(CreateAttribute(
            name: "AccountName",
            typeId: GetTypeId(package, "string"),
            parentId: accountDto.Id,
            externalReference: "account.json.accountName"));
        
        package.Classes.Add(accountDto);
        return package;
    }

    private static ElementPersistable CreateClass(
        string name,
        string packageId,
        string externalReference,
        string specializationType,
        string specializationTypeId)
    {
        var classElement = ElementPersistable.Create(
            specializationType: specializationType,
            specializationTypeId: specializationTypeId,
            name: name,
            parentId: packageId,
            externalReference: externalReference);
        
        classElement.PackageId = packageId;
        classElement.ChildElements = new List<ElementPersistable>();
        
        return classElement;
    }

    private static ElementPersistable CreateAttribute(
        string name,
        string typeId,
        string parentId,
        string externalReference)
    {
        var attribute = ElementPersistable.Create(
            specializationType: "Attribute",
            specializationTypeId: "0090fb93-483e-49c5-84d7-adab0b58bdce",
            name: name,
            parentId: parentId,
            externalReference: externalReference);
        
        attribute.TypeReference = new TypeReferencePersistable
        {
            TypeId = typeId
        };
        
        return attribute;
    }

    private static string GetTypeId(PackageModelPersistable package, string typeName)
    {
        return package.Classes
            .First(c => c.SpecializationType == TypeDefinitionModel.SpecializationType && 
                       string.Equals(c.Name, typeName, StringComparison.OrdinalIgnoreCase))
            .Id;
    }

    private static ElementPersistable CreateTypeDefinition(string typeName, string packageId, string packageName)
    {
        var typeDefinition = ElementPersistable.Create(
            specializationType: TypeDefinitionModel.SpecializationType,
            specializationTypeId: TypeDefinitionModel.SpecializationTypeId,
            name: typeName,
            parentId: null,
            externalReference: null);

        typeDefinition.PackageId = packageId;
        typeDefinition.PackageName = packageName;

        return typeDefinition;
    }
}
