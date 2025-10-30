using Intent.Modules.Json.Importer.Importer;
using Intent.Modules.Json.Importer.Tests.TestData;
using Shouldly;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Json.Importer.Tests;

/// <summary>
/// Snapshot tests for Services DTOs profile mapping validation.
/// Tests use Verify library to capture and compare full mapping output.
/// </summary>
public class ServicesDtosVisitorMappingTests
{
    [Fact]
    public Task MapServices_SimpleDto_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.ServicesProfile(JsonDocuments.ServicesFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.ServicesFolder(), "simple-dto.json")]);

        // Assert
        result.Elements.Count.ShouldBe(1);
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("simple-dto").ScrubInlineGuids();
    }

    [Fact]
    public Task MapServices_DtoWithNestedObject_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.ServicesProfile(JsonDocuments.ServicesFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.ServicesFolder(), "dto-with-nested-object.json")]);

        // Assert
        result.Elements.Count.ShouldBe(2);
        result.Associations.ShouldBeEmpty();
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("dto-with-nested-object").ScrubInlineGuids();
    }

    [Fact]
    public Task MapServices_DtoWithCollection_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.ServicesProfile(JsonDocuments.ServicesFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.ServicesFolder(), "dto-with-collection.json")]);

        // Assert
        result.Elements.Count.ShouldBe(2);
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("dto-with-collection").ScrubInlineGuids();
    }

    [Fact]
    public Task MapServices_ComplexDto_AllTypes_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.ServicesProfile(JsonDocuments.ServicesFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.ServicesFolder(), "simple-dto.json")]);

        // Assert
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("complex-dto-all-types").ScrubInlineGuids();
    }
}
