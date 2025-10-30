using Intent.Modules.Json.Importer.Importer;
using Intent.Modules.Json.Importer.Tests.TestData;
using Shouldly;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Json.Importer.Tests;

/// <summary>
/// Snapshot tests for Eventing Messages profile mapping validation.
/// Tests use Verify library to capture and compare full mapping output.
/// </summary>
public class EventingMessagesVisitorMappingTests
{
    [Fact]
    public Task MapEventing_SimpleMessage_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.EventingProfile(JsonDocuments.EventingFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.EventingFolder(), "simple-message.json")]);

        // Assert
        result.Elements.Count.ShouldBe(1);
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("simple-message").ScrubInlineGuids();
    }

    [Fact]
    public Task MapEventing_MessageWithNestedDto_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.EventingProfile(JsonDocuments.EventingFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.EventingFolder(), "message-with-nested-dto.json")]);

        // Assert
        result.Elements.Count.ShouldBe(2);
        result.Associations.ShouldBeEmpty();
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("message-with-nested-dto").ScrubInlineGuids();
    }

    [Fact]
    public Task MapEventing_MessageWithCollection_ShouldMatchSnapshot()
    {
        // Arrange
        var package = PackageModels.WithServicesTypes();
        var config = ImportConfigurations.EventingProfile(JsonDocuments.EventingFolder());

        // Act
        var result = JsonPersistableFactory.GetPersistables(config, [package], [Path.Combine(JsonDocuments.EventingFolder(), "message-with-collection.json")]);

        // Assert
        var snapshot = SnapshotBuilder.BuildSnapshot(result);
        return Verify(snapshot).UseParameters("message-with-collection").ScrubInlineGuids();
    }
}
