using Intent.Modules.Json.Importer.Importer;
using Intent.Modules.Json.Importer.Tests.TestData;
using Shouldly;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Json.Importer.Tests;

/// <summary>
/// Snapshot tests for Domain profile mapping validation.
/// Tests use Verify library to capture and compare full mapping output.
/// </summary>
public class DomainVisitorMappingTests
{
    [Fact]
    public Task MapDomain_SimpleClass_BasicProperties_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [JsonDocuments.SimpleCustomerFile()]);

        // Assert
        result.Elements.Count.ShouldBeGreaterThan(0);
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("simple-class").ScrubInlineGuids();
    }

    [Fact]
    public Task MapDomain_NestedObject_CreatesAssociation_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "customer-with-address.json")]);

        // Assert
        result.Elements.Count.ShouldBe(2);
        result.Associations.ShouldHaveSingleItem();
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("nested-object").ScrubInlineGuids();
    }

    [Fact]
    public Task MapDomain_ArrayOfObjects_CollectionAssociation_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "customer-with-orders.json")]);

        // Assert
        result.Elements.Count.ShouldBe(2);
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("collection-association").ScrubInlineGuids();
    }

    [Fact]
    public Task MapDomain_AllPrimitiveTypes_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "all-primitive-types.json")]);

        // Assert
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("all-primitives").ScrubInlineGuids();
    }

    [Fact]
    public Task MapDomain_NestedCollections_ComplexStructure_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "nested-collections.json")]);

        // Assert
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("nested-collections").ScrubInlineGuids();
    }

    [Fact]
    public Task MapDomain_EmptyArray_WithRemarks_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.DomainFolder(), "empty-array.json")]);

        // Assert
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("empty-array").ScrubInlineGuids();
    }
}
