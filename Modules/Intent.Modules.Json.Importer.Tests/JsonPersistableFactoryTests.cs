using Intent.Modules.Json.Importer.Importer;
using Intent.Modules.Json.Importer.Tests.TestData;
using Shouldly;
using Xunit;

namespace Intent.Modules.Json.Importer.Tests;

/// <summary>
/// Behavioural tests for JsonPersistableFactory using explicit assertions.
/// Tests follow Arrange/Act/Assert pattern and focus on state changes.
/// </summary>
public class JsonPersistableFactoryTests
{
    [Fact]
    public void GetPersistables_SimpleJson_CreatesClassWithAttributes()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.SimpleCustomerFile()]);

        // Assert
        result.Elements.Count.ShouldBe(1);
        var customerClass = result.Elements.First();
        customerClass.Name.ShouldBe("SimpleCustomer");
        customerClass.SpecializationType.ShouldBe("Class");
        customerClass.ExternalReference.ShouldBe("simple-customer.json");
        customerClass.ChildElements.Count.ShouldBe(3);
        customerClass.ChildElements.ShouldContain(e => e.Name == "Id");
        customerClass.ChildElements.ShouldContain(e => e.Name == "Name");
        customerClass.ChildElements.ShouldContain(e => e.Name == "Email");
    }

    [Fact]
    public void GetPersistables_NestedObject_CreatesAssociation()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "customer-with-address.json")]);

        // Assert
        result.Elements.Count.ShouldBe(2);
        result.Elements.ShouldContain(e => e.Name == "CustomerWithAddress");
        result.Elements.ShouldContain(e => e.Name == "Address");
        
        result.Associations.ShouldHaveSingleItem();
        var association = result.Associations.First();
        association.SourceEnd.Name.ShouldBe("CustomerWithAddress");
        association.TargetEnd.Name.ShouldBe("Address");
        association.TargetEnd.TypeReference.IsCollection.ShouldBeFalse();
        association.TargetEnd.TypeReference.IsNavigable.ShouldBeTrue();
    }

    [Fact]
    public void GetPersistables_ArrayOfObjects_CreatesCollectionAssociation()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "customer-with-orders.json")]);

        // Assert
        result.Elements.Count.ShouldBe(2);
        result.Associations.ShouldHaveSingleItem();
        var association = result.Associations.First();
        association.TargetEnd.Name.ShouldBe("Orders");
        association.TargetEnd.TypeReference.IsCollection.ShouldBeTrue();
        
        var orderClass = result.Elements.First(e => e.Name == "Order");
        orderClass.ChildElements.Count.ShouldBe(3);
    }

    [Fact]
    public void GetPersistables_DomainProfile_AddsPrimaryKeyStereotypeToRootId()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.SimpleCustomerFile()]);

        // Assert
        var customerClass = result.Elements.First(e => e.Name == "SimpleCustomer");
        var idAttribute = customerClass.ChildElements.First(e => e.Name == "Id");
        idAttribute.Stereotypes.ShouldHaveSingleItem();
        idAttribute.Stereotypes.First().Name.ShouldBe("Primary Key");
        idAttribute.Metadata.ShouldContain(m => m.Key == "is-managed-key" && m.Value == "true");
    }

    [Fact]
    public void GetPersistables_EventingProfile_CreatesMessage()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.EventingProfile(JsonDocuments.EventingFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.EventingFolder(), "simple-message.json")]);

        // Assert
        result.Elements.Count.ShouldBe(1);
        var message = result.Elements.First();
        message.Name.ShouldBe("SimpleMessage");
        message.SpecializationType.ShouldBe("Message");
        message.ChildElements.Count.ShouldBe(4);
    }

    [Fact]
    public void GetPersistables_EventingProfile_NestedObject_CreatesEventingDto()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.EventingProfile(JsonDocuments.EventingFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.EventingFolder(), "message-with-nested-dto.json")]);

        // Assert
        result.Elements.Count.ShouldBe(2);
        result.Elements.ShouldContain(e => e.SpecializationType == "Message");
        result.Elements.ShouldContain(e => e.SpecializationType == "Eventing DTO");
        result.Associations.ShouldBeEmpty();
        
        var message = result.Elements.First(e => e.SpecializationType == "Message");
        message.ChildElements.ShouldContain(e => e.Name == "OrderDetails" && e.SpecializationType == "Property");
    }

    [Fact]
    public void GetPersistables_ServicesProfile_CreatesDto()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.ServicesProfile(JsonDocuments.ServicesFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.ServicesFolder(), "simple-dto.json")]);

        // Assert
        result.Elements.Count.ShouldBe(1);
        var dto = result.Elements.First();
        dto.Name.ShouldBe("SimpleDtoDto");
        dto.SpecializationType.ShouldBe("DTO");
        dto.ChildElements.All(c => c.SpecializationType == "DTO-Field").ShouldBeTrue();
    }

    [Fact]
    public void GetPersistables_ServicesProfile_NestedObject_CreatesNestedDto()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.ServicesProfile(JsonDocuments.ServicesFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.ServicesFolder(), "dto-with-nested-object.json")]);

        // Assert
        result.Elements.Count.ShouldBe(2);
        result.Elements.All(e => e.SpecializationType == "DTO").ShouldBeTrue();
        result.Elements.ShouldContain(e => e.Name == "AddressDto");
        result.Associations.ShouldBeEmpty();
    }

    [Fact]
    public void GetPersistables_EmptyArray_AddsRemarkAboutUnknownType()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "empty-array.json")]);

        // Assert
        var entity = result.Elements.First();
        var tagsAttribute = entity.ChildElements.First(e => e.Name == "Tags");
        tagsAttribute.TypeReference.Comment.ShouldNotBeNullOrEmpty();
        tagsAttribute.TypeReference.Comment.ShouldContain("Array contained no elements");
    }

    [Fact]
    public void GetPersistables_NullValue_AddsRemarkAboutUnknownType()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "null-value.json")]);

        // Assert
        var entity = result.Elements.First();
        var middleNameAttribute = entity.ChildElements.First(e => e.Name == "MiddleName");
        middleNameAttribute.TypeReference.Comment.ShouldNotBeNullOrEmpty();
        middleNameAttribute.TypeReference.Comment.ShouldContain("Unable to determine type");
    }

    [Fact]
    public void GetPersistables_PascalCaseConvention_ConvertsNames()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.ServicesFolder()).WithCasing(CasingConvention.PascalCase);

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.ServicesFolder(), "simple-dto.json")]);

        // Assert
        var entity = result.Elements.First();
        entity.ChildElements.ShouldContain(e => e.Name == "FirstName");
        entity.ChildElements.ShouldContain(e => e.Name == "LastName");
        entity.ChildElements.ShouldContain(e => e.Name == "IsActive");
    }

    [Fact]
    public void GetPersistables_AsIsConvention_PreservesNames()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.ServicesFolder()).WithCasing(CasingConvention.AsIs);

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.ServicesFolder(), "simple-dto.json")]);

        // Assert
        var entity = result.Elements.First();
        entity.ChildElements.ShouldContain(e => e.Name == "firstName");
        entity.ChildElements.ShouldContain(e => e.Name == "lastName");
        entity.ChildElements.ShouldContain(e => e.Name == "isActive");
    }

    [Fact]
    public void GetPersistables_MultipleFilesInRootFolder_ProcessesAll()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act - Don't pass specific files so it scans all JSON files in the root folder
        var result = JsonPersistableFactory.GetPersistables(config, [package]);

        // Assert - Should process all JSON files in the root folder (not subfolders)
        result.Elements.Count.ShouldBeGreaterThanOrEqualTo(8); // All JSON files in root folder
        result.Elements.ShouldContain(e => e.Name == "SimpleCustomer");
        result.Elements.ShouldContain(e => e.Name == "Invoice");
        result.Elements.ShouldContain(e => e.Name == "CustomerWithAddress");
    }

    [Fact]
    public void GetPersistables_MultipleFiles_CreatesMultipleElements()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.SimpleCustomerFile(), JsonDocuments.InvoiceFile()]);

        // Assert
        result.Elements.ShouldContain(e => e.Name == "SimpleCustomer" && e.ExternalReference == "simple-customer.json");
        result.Elements.ShouldContain(e => e.Name == "Invoice" && e.ExternalReference == "invoice.json");
    }

    [Fact]
    public void GetPersistables_SelectedFiles_ProcessesOnlySelectedFiles()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.SimpleCustomerFile()]);

        // Assert
        result.Elements.Count.ShouldBe(1);
        result.Elements.First().ExternalReference.ShouldBe("simple-customer.json");
    }

    [Fact]
    public void GetPersistables_AllPrimitiveTypes_MapsCorrectly()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "all-primitive-types.json")]);

        // Assert
        var entity = result.Elements.First();
        entity.ChildElements.Count.ShouldBe(6);
        entity.ChildElements.ShouldContain(e => e.Name == "Id");
        entity.ChildElements.ShouldContain(e => e.Name == "Name");
        entity.ChildElements.ShouldContain(e => e.Name == "Age");
        entity.ChildElements.ShouldContain(e => e.Name == "IsActive");
        entity.ChildElements.ShouldContain(e => e.Name == "Balance");
        entity.ChildElements.ShouldContain(e => e.Name == "CreatedAt");
    }
}
