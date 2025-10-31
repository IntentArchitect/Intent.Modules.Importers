using System.CommandLine;
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
            id: "e1bb6425-a920-4c90-998f-209c259a6d18",
            name: "SimpleCustomer",
            packageId: package.Id,
            externalReference: "simple-customer.json",
            specializationType: "Class",
            specializationTypeId: "04e12b51-ed12-42a3-9667-a6aa81bb6d10");
        
        // Add attributes that match simple customer JSON
        customerClass.ChildElements.Add(CreateAttribute(
            id: "1aa164ce-7369-4991-a94d-25217c6a86f6",
            name: "Id",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Id"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            id: "af83e34c-8cc6-4750-96cd-6bf31bc731ed",
            name: "Name",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Name"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            id: "f4c404cc-59d7-489c-b98e-1f24d5586728",
            name: "Email",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Email"));
        
        package.Classes.Add(customerClass);
        return package;
    }

    /// <summary>
    /// Creates a domain package with an existing customer class at the specified path.
    /// This is used for testing re-import scenarios where the import file path must match
    /// the ExternalReference of the existing package model elements.
    /// </summary>
    private static PackageModelPersistable WithExistingCustomerFromPath(string folderName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderName);
        
        var package = WithDomainTypes();

        string filePath = $"{folderName}/simple-customer.json";
        
        var folder = ElementPersistable.Create(
            specializationType: "Folder",
            specializationTypeId: "4d95d53a-8855-4f35-aa82-e312643f5c5f",
            name: folderName,
            parentId: package.Id,
            externalReference: $"folder:Customers");
        folder.Id = "b6a156be-19c5-476a-83fd-ca6ddd76c3b5";
        package.Classes.Add(folder);
        
        var customerClass = CreateClass(
            id: "ef412c0e-1575-43b1-85bf-2ae6852e650d",
            name: "SimpleCustomer",
            packageId: folder.Id,
            externalReference: filePath,
            specializationType: "Class",
            specializationTypeId: "04e12b51-ed12-42a3-9667-a6aa81bb6d10");
        
        // Add attributes that match simple customer JSON
        customerClass.ChildElements.Add(CreateAttribute(
            id: "b5a03fe8-bbeb-41d2-aa90-54df772a79ff",
            name: "Id",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: $"{filePath}.Id"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            id: "47293caa-3905-45fd-a525-97288eb14534",
            name: "Name",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: $"{filePath}.Name"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            id: "d009819d-af3c-4686-8cde-25e465e4724f",
            name: "Email",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: $"{filePath}.Email"));
        
        package.Classes.Add(customerClass);
        return package;
    }

    public static PackageModelPersistable WithExistingCustomerWithExtraPropertyFromPath()
    {
        return WithExistingCustomerFromPath("ExtraProperty");
    }
    
    public static PackageModelPersistable WithExistingCustomerWithMissingPropertyFromPath()
    {
        return WithExistingCustomerFromPath("MissingProperty");
    }

    /// <summary>
    /// Creates a domain package with existing Customer and Invoice classes.
    /// Used for testing merge scenarios with multiple existing elements.
    /// </summary>
    public static PackageModelPersistable WithCustomerAndInvoice()
    {
        var package = WithDomainTypes();
        
        var customerClass = CreateClass(
            id: "7aba8b08-45fc-4f4c-b0fe-5767ed212ed3",
            name: "SimpleCustomer",
            packageId: package.Id,
            externalReference: "simple-customer.json",
            specializationType: "Class",
            specializationTypeId: "04e12b51-ed12-42a3-9667-a6aa81bb6d10");
        
        customerClass.ChildElements.Add(CreateAttribute(
            id: "56b7a24d-defc-497f-9fe9-904fd62f46cf",
            name: "Id",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Id"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            id: "f09d3895-61f0-462e-a209-a9d2f5f73018",
            name: "Name",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Name"));
        
        customerClass.ChildElements.Add(CreateAttribute(
            id: "18e73681-56d7-44b1-814e-68c816afd216",
            name: "Email",
            typeId: GetTypeId(package, "string"),
            parentId: customerClass.Id,
            externalReference: "simple-customer.json.Email"));
        
        var invoiceClass = CreateClass(
            id: "3d343600-2a02-4a0c-a92a-69a0dd4432f4",
            name: "Invoice",
            packageId: package.Id,
            externalReference: "invoice.json",
            specializationType: "Class",
            specializationTypeId: "04e12b51-ed12-42a3-9667-a6aa81bb6d10");
        
        invoiceClass.ChildElements.Add(CreateAttribute(
            id: "0c8f3068-8918-4a6e-864e-2db25f846148",
            name: "Id",
            typeId: GetTypeId(package, "string"),
            parentId: invoiceClass.Id,
            externalReference: "invoice.json.Id"));
        
        invoiceClass.ChildElements.Add(CreateAttribute(
            id: "50642e0b-b87c-4492-a4c1-3d304179e080",
            name: "InvoiceNumber",
            typeId: GetTypeId(package, "string"),
            parentId: invoiceClass.Id,
            externalReference: "invoice.json.InvoiceNumber"));
        
        invoiceClass.ChildElements.Add(CreateAttribute(
            id: "fca00259-4fde-4013-ae48-216667751b6d",
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
            id: "b0b86a5f-251b-43a7-9d46-8098466e5f2a",
            name: "Account",
            packageId: package.Id,
            externalReference: "account.json",
            specializationType: "DTO",
            specializationTypeId: "c2188e49-2989-43f8-b1a4-3263d56af4f7");
        
        accountDto.ChildElements.Add(CreateAttribute(
            id: "8b86daf2-2915-4006-b9d5-fbf13f5caa95",
            name: "AccountId",
            typeId: GetTypeId(package, "guid"),
            parentId: accountDto.Id,
            externalReference: "account.json.accountId"));
        
        accountDto.ChildElements.Add(CreateAttribute(
            id: "1fe08e9b-66f7-4ad4-be62-fd4bab3bd58f",
            name: "AccountName",
            typeId: GetTypeId(package, "string"),
            parentId: accountDto.Id,
            externalReference: "account.json.accountName"));
        
        package.Classes.Add(accountDto);
        return package;
    }

    private static ElementPersistable CreateClass(
        string id,
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

        classElement.Id = id;
        classElement.PackageId = packageId;
        classElement.ChildElements = new List<ElementPersistable>();
        
        return classElement;
    }

    private static ElementPersistable CreateAttribute(
        string id,
        string name,
        string typeId,
        string parentId,
        string externalReference)
    {
        var attribute = ElementPersistable.Create(
            specializationType: "Attribute",
            specializationTypeId: AttributeModel.SpecializationTypeId,
            name: name,
            parentId: parentId,
            externalReference: externalReference);
        attribute.Id = id;
        
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
