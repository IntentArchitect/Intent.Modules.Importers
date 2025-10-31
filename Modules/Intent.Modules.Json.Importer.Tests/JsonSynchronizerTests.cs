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
        var customerClassBefore = GetClasses(package).Single(c => c.Name == "SimpleCustomer");
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
        var customerClassAfter = GetClasses(package).Single(c => c.Name == "SimpleCustomer");
        customerClassAfter.Id.ShouldBe(classIdBefore, "Class ID should remain unchanged");
        customerClassAfter.ChildElements.Count.ShouldBe(3);
        customerClassAfter.ChildElements.Select(a => a.Id).ShouldBe(attributeIdsBefore, "Attribute IDs should remain unchanged");
    }

    [Fact]
    public void Synchronize_NewPropertyAdded_AddsNewAttribute()
    {
        // Arrange - Create package with customer that has ExternalReference matching the import file path
        var package = PackageModels.WithExistingCustomerWithExtraPropertyFromPath();
        var customerClassBefore = GetClasses(package).Single(c => c.Name == "SimpleCustomer");
        customerClassBefore.ChildElements.Count.ShouldBe(3, "Should start with 3 attributes");
        
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act - Re-import the same file which now has 4 properties
        var persistables = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.CustomerWithExtraPropertyFile()]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var customerClassAfter = GetClasses(package).Single(c => c.Name == "SimpleCustomer");
        customerClassAfter.ChildElements.Count.ShouldBe(4, "Should now have 4 attributes");
        GetAttributeNames(customerClassAfter).ShouldBe(new[] { "Id", "Name", "Email", "PhoneNumber" });
    }

    [Fact]
    public void Synchronize_PropertyRemovedWithoutDeleteExtra_KeepsAttribute()
    {
        // Arrange - Create package with customer that has ExternalReference matching the import file path
        var package = PackageModels.WithExistingCustomerWithMissingPropertyFromPath();
        var customerClassBefore = GetClasses(package).Single(c => c.Name == "SimpleCustomer");
        customerClassBefore.ChildElements.Count.ShouldBe(3);
        
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act - Re-import the same file which now has only 1 property (Id)
        var persistables = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.CustomerWithMissingPropertyFile()]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false, // Keep orphaned attributes
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var customerClassAfter = GetClasses(package).Single(c => c.Name == "SimpleCustomer");
        customerClassAfter.ChildElements.Count.ShouldBe(3, "Should still have 3 attributes (no deletion)");
        GetAttributeNames(customerClassAfter).ShouldBe(new[] { "Id", "Name", "Email" });
    }

    [Fact]
    public void Synchronize_PropertyRemovedWithDeleteExtra_RemovesAttribute()
    {
        // Arrange - Create package with customer that has ExternalReference matching the import file path
        var package = PackageModels.WithExistingCustomerWithMissingPropertyFromPath();
        var customerClassBefore = GetClasses(package).Single(c => c.Name == "SimpleCustomer");
        customerClassBefore.ChildElements.Count.ShouldBe(3);
        
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act - Re-import the same file which now has only 1 property (Id)
        var persistables = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.CustomerWithMissingPropertyFile()]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: true, // Delete orphaned attributes
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var customerClassAfter = GetClasses(package).Single(c => c.Name == "SimpleCustomer");
        customerClassAfter.ChildElements.Count.ShouldBe(1, "Should only have 1 attribute (Name and Email deleted)");
        GetAttributeNames(customerClassAfter).ShouldBe(new[] { "Id" });
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
        classes.ShouldContain(c => c.Name == "SimpleCustomer");
        classes.ShouldContain(c => c.Name == "Product");
    }

    [Fact]
    public void Synchronize_ReimportSameFileTwice_ShouldNotThrowDuplicateKeyException()
    {
        // Arrange - This might be what's happening in the DVT scenario
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        var userFile = Path.Combine(JsonDocuments.DomainFolder(), "user.json");

        // Act - Import the same file twice (simulating a re-import scenario)
        var exception = Record.Exception(() =>
        {
            // First import
            var persistables1 = JsonPersistableFactory.GetPersistables(
                config,
                [package],
                [userFile]);

            Synchronizer.Execute(
                targetPackage: package,
                parentFolderId: package.Id,
                persistables: persistables1,
                deleteExtra: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge);

            // Second import of the same file - this is where the error might occur
            var persistables2 = JsonPersistableFactory.GetPersistables(
                config,
                [package],
                [userFile]);

            Synchronizer.Execute(
                targetPackage: package,
                parentFolderId: package.Id,
                persistables: persistables2,
                deleteExtra: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge);
        });

        // Assert
        exception.ShouldBeNull("Should not throw duplicate key exception when reimporting the same file");
    }

    [Fact]
    public void Import_SameFileInListTwice_ShouldDeduplicateAndNotThrow()
    {
        // Arrange - This tests if the file is accidentally in the list twice
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        var userFile = Path.Combine(JsonDocuments.DomainFolder(), "user.json");

        // Act - Pass the same file twice in the list (this should be deduplicated)
        var exception = Record.Exception(() =>
        {
            var persistables = JsonPersistableFactory.GetPersistables(
                config,
                [package],
                [userFile, userFile]); // Same file twice!

            Synchronizer.Execute(
                targetPackage: package,
                parentFolderId: package.Id,
                persistables: persistables,
                deleteExtra: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge);
        });

        // Assert
        exception.ShouldBeNull("Should deduplicate files and not throw duplicate key exception");
    }

    [Fact]
    public void Synchronize_UserJsonWithIdentities_ShouldNotDuplicateFields()
    {
        // Arrange - This tests the DVT scenario
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        var userFile = Path.Combine(JsonDocuments.DomainFolder(), "user.json");

        // Act
        var persistables = JsonPersistableFactory.GetPersistables(
            config,
            [package],
            [userFile]);

        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert - Check for duplicate fields in User class
        var userClass = GetClasses(package).FirstOrDefault(c => c.Name == "User");
        userClass.ShouldNotBeNull("User class should exist after import");

        var fieldNames = userClass.ChildElements.Select(e => e.Name).ToList();
        var duplicates = fieldNames.GroupBy(name => name)
            .Where(g => g.Count() > 1)
            .Select(g => $"{g.Key} (appears {g.Count()} times)")
            .ToList();

        duplicates.ShouldBeEmpty($"Found duplicate fields in User class: {string.Join(", ", duplicates)}");

        // Also check the Identity class
        var identityClass = GetClasses(package).FirstOrDefault(c => c.Name.Contains("Identity"));
        if (identityClass != null)
        {
            var identityFieldNames = identityClass.ChildElements.Select(e => e.Name).ToList();
            var identityDuplicates = identityFieldNames.GroupBy(name => name)
                .Where(g => g.Count() > 1)
                .Select(g => $"{g.Key} (appears {g.Count()} times)")
                .ToList();

            identityDuplicates.ShouldBeEmpty($"Found duplicate fields in {identityClass.Name}: {string.Join(", ", identityDuplicates)}");
        }
    }

    private static IEnumerable<ElementPersistable> GetClasses(PackageModelPersistable package) =>
        package.Classes.Where(c =>
            string.Equals(c.SpecializationType, ClassModel.SpecializationType, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<string> GetAttributeNames(ElementPersistable element) =>
        element.ChildElements.Where(p => p.SpecializationTypeId == AttributeModel.SpecializationTypeId)
            .Select(a => a.Name);
}
