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

        // Check that Identity classes from both files have different ExternalReferences
        var identityClasses = persistables.Elements
            .Where(e => e.Name == "Identity")
            .ToList();

        identityClasses.Count.ShouldBe(2, "Should have two Identity classes, one from each file");

        var externalRefs = identityClasses.Select(c => c.ExternalReference).ToList();
        externalRefs.ShouldContain(er => er.Contains("user.json"), "Should have Identity from user.json");
        externalRefs.ShouldContain(er => er.Contains("users-by-email-response.json"), "Should have Identity from users-by-email-response.json");

        // Verify they are distinct
        externalRefs.Distinct().Count().ShouldBe(2, "ExternalReferences should be unique");
    }
}
