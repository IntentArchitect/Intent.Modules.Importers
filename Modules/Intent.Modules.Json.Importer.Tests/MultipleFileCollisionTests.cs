using Intent.Modules.Json.Importer.Importer;
using Intent.Modules.Json.Importer.Tests.TestData;
using Shouldly;

namespace Intent.Modules.Json.Importer.Tests;

/// <summary>
/// Tests for multiple JSON files with similar structures that could cause ExternalReference collisions.
/// </summary>
public class MultipleFileCollisionTests
{
    [Fact]
    public void ImportMultipleFilesWithIdentitiesCollection_ShouldNotCauseCollision()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        var userFile = Path.Combine(JsonDocuments.DomainFolder(), "user.json");
        var createUserFile = Path.Combine(JsonDocuments.DomainFolder(), "create_user_2.json");
        var usersByEmailFile = Path.Combine(JsonDocuments.DomainFolder(), "users-by-email-response.json");

        // Act - Import all three files that have similar structures
        var exception = Record.Exception(() =>
        {
            var persistables = JsonPersistableFactory.GetPersistables(
                config,
                [package],
                [userFile, createUserFile, usersByEmailFile]);
        });

        // Assert
        exception.ShouldBeNull("Should not throw duplicate key exception when importing multiple files with similar nested structures");
    }

    [Fact]
    public void ImportUserAndUsersByEmailResponse_CheckExternalReferences()
    {
        // Arrange
        var package = PackageModels.WithDomainTypes();
        var config = ImportConfigurations.DomainProfile(JsonDocuments.DomainFolder());

        var userFile = Path.Combine(JsonDocuments.DomainFolder(), "user.json");
        var usersByEmailFile = Path.Combine(JsonDocuments.DomainFolder(), "users-by-email-response.json");

        // Act
        var persistables = JsonPersistableFactory.GetPersistables(
            config,
            [package],
            [userFile, usersByEmailFile]);

        // Assert
        persistables.Elements.Count.ShouldBeGreaterThan(0);

        // Check that Identity-derived classes from both files have different ExternalReferences
        var identityClasses = persistables.Elements
            .Where(e => e.ExternalReference?.Contains(".identities[0]", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        identityClasses.Count.ShouldBe(2, "Should have two Identity-derived classes, one from each file");

        var orderedNames = identityClasses.Select(c => c.Name).OrderBy(x => x).ToArray();
        orderedNames.ShouldBe(new[] { "UserIdentity", "UsersByEmailResponseIdentity" });

        var externalRefs = identityClasses.Select(c => c.ExternalReference).OrderBy(x => x).ToArray();
        externalRefs.ShouldBe(new[]
        {
            "user.json.identities[0]",
            "users-by-email-response.json.identities[0]"
        });
    }
}
