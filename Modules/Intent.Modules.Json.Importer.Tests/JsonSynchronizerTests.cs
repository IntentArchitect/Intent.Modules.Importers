using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Json.Importer.Importer;
using Intent.Modules.Json.Importer.Tests.TestData;
using Shouldly;

namespace Intent.Modules.Json.Importer.Tests;

/// <summary>
/// Tests for merging/re-importing JSON documents into packages with existing elements.
/// Validates synchronization behavior via ExternalReference matching using Intent.MetadataSynchronizer.
/// </summary>
public class JsonSynchronizerTests
{
    [Fact]
    public void Synchronize_SameDataReimported_RemainsIdempotent()
    {
        // Arrange
        var package = PackageModels.WithExistingCustomer();
        var customerClassBefore = GetClasses(package).Single(c => c.Name == "Customer");
        var classIdBefore = customerClassBefore.Id;
        var attributeIdsBefore = customerClassBefore.ChildElements.Select(a => a.Id).ToList();
        
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var persistables = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.SimpleCustomerFile()]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var customerClassAfter = GetClasses(package).Single(c => c.Name == "Customer");
        customerClassAfter.Id.ShouldBe(classIdBefore, "Class ID should remain unchanged");
        customerClassAfter.ChildElements.Count.ShouldBe(2);
        customerClassAfter.ChildElements.Select(a => a.Id).ShouldBe(attributeIdsBefore, "Attribute IDs should remain unchanged");
    }

    [Fact]
    public void Synchronize_NewPropertyAdded_AddsNewAttribute()
    {
        // Arrange
        var package = PackageModels.WithExistingCustomer();
        var customerClassBefore = GetClasses(package).Single(c => c.Name == "Customer");
        customerClassBefore.ChildElements.Count.ShouldBe(2, "Should start with 2 attributes");
        
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var persistables = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.CustomerWithExtraPropertyFile()]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var customerClassAfter = GetClasses(package).Single(c => c.Name == "Customer");
        customerClassAfter.ChildElements.Count.ShouldBe(3, "Should now have 3 attributes");
        GetAttributeNames(customerClassAfter).ShouldBe(new[] { "CustomerId", "Email", "PhoneNumber" });
    }

    [Fact]
    public void Synchronize_PropertyRemovedWithoutDeleteExtra_KeepsAttribute()
    {
        // Arrange
        var package = PackageModels.WithExistingCustomer();
        var customerClassBefore = GetClasses(package).Single(c => c.Name == "Customer");
        customerClassBefore.ChildElements.Count.ShouldBe(2);
        
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var persistables = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.CustomerWithMissingPropertyFile()]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false, // Keep orphaned attributes
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var customerClassAfter = GetClasses(package).Single(c => c.Name == "Customer");
        customerClassAfter.ChildElements.Count.ShouldBe(2, "Should still have 2 attributes (no deletion)");
        GetAttributeNames(customerClassAfter).ShouldBe(new[] { "CustomerId", "Email" });
    }

    [Fact]
    public void Synchronize_PropertyRemovedWithDeleteExtra_RemovesAttribute()
    {
        // Arrange
        var package = PackageModels.WithExistingCustomer();
        var customerClassBefore = GetClasses(package).Single(c => c.Name == "Customer");
        customerClassBefore.ChildElements.Count.ShouldBe(2);
        
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var persistables = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.CustomerWithMissingPropertyFile()]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: true, // Delete orphaned attributes
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var customerClassAfter = GetClasses(package).Single(c => c.Name == "Customer");
        customerClassAfter.ChildElements.Count.ShouldBe(1, "Should only have 1 attribute (email deleted)");
        GetAttributeNames(customerClassAfter).ShouldBe(new[] { "CustomerId" });
    }

    [Fact]
    public void Synchronize_NewFileAdded_AddsOnlyNewClass()
    {
        // Arrange
        var package = PackageModels.WithExistingCustomer();
        GetClasses(package).Count().ShouldBe(1, "Should start with 1 class");
        
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var persistables = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.ProductFile()]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var classes = GetClasses(package).ToList();
        classes.Count.ShouldBe(2, "Should now have 2 classes");
        classes.ShouldContain(c => c.Name == "Customer");
        classes.ShouldContain(c => c.Name == "Product");
    }

    private static IEnumerable<ElementPersistable> GetClasses(PackageModelPersistable package) =>
        package.Classes.Where(c =>
            string.Equals(c.SpecializationType, ClassModel.SpecializationType, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<string> GetAttributeNames(ElementPersistable element) =>
        element.ChildElements.Where(p => p.SpecializationTypeId == AttributeModel.SpecializationTypeId)
            .Select(a => a.Name);
}
