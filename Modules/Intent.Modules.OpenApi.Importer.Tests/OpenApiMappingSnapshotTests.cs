using Intent.Modules.OpenApi.Importer.Importer;
using Intent.Modules.OpenApi.Importer.Tests.TestData;

namespace Intent.Modules.OpenApi.Importer.Tests;

/// <summary>
/// Snapshot tests for OpenAPI import mappings using Verify framework.
/// These tests verify complete object structures and catch regressions across entire import results.
/// Naming convention: Map{Component}_{Feature}_{Scenario}_ShouldMatchSnapshot
/// </summary>
public class OpenApiMappingSnapshotTests
{
    [Fact]
    public Task MapOpenApi_PetStore_CQRS_ShouldMatchSnapshot()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("pet-store.yaml");
        var config = ImportConfigurations.CQRSMode();
        var packages = new[] { PackageModels.WithTypeDefinitions() };

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        return Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("MapOpenApi_PetStore_CQRS_ShouldMatchSnapshot");
    }

    [Fact]
    public Task MapOpenApi_PetStore_Service_ShouldMatchSnapshot()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("pet-store.yaml");
        var config = ImportConfigurations.ServiceMode();
        var packages = new[] { PackageModels.WithTypeDefinitions() };

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        return Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("MapOpenApi_PetStore_Service_ShouldMatchSnapshot");
    }

    [Fact]
    public Task MapOpenApi_WithEnum_CreatesEnumElements_ShouldMatchSnapshot()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("with-enum.json");
        var config = ImportConfigurations.CQRSMode();
        var packages = new[] { PackageModels.WithTypeDefinitions() };

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        return Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("MapOpenApi_WithEnum_CreatesEnumElements_ShouldMatchSnapshot");
    }

    [Fact]
    public Task MapOpenApi_WithQuery_MapsQueryParameters_ShouldMatchSnapshot()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("with-query.json");
        var config = ImportConfigurations.CQRSMode();
        var packages = new[] { PackageModels.WithTypeDefinitions() };

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        return Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("MapOpenApi_WithQuery_MapsQueryParameters_ShouldMatchSnapshot");
    }

    [Fact]
    public Task MapOpenApi_SwaggerSample_CreatesDTOs_ShouldMatchSnapshot()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("swagger-sample.json");
        var config = ImportConfigurations.CQRSMode();
        var packages = new[] { PackageModels.WithTypeDefinitions() };

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        return Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("MapOpenApi_SwaggerSample_CreatesDTOs_ShouldMatchSnapshot");
    }

    [Fact]
    public Task MapOpenApi_SecuredSpec_AddsSecurityStereotypes_ShouldMatchSnapshot()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("intent-generated-secured.json");
        var config = ImportConfigurations.CQRSMode();
        var packages = new[] { PackageModels.WithTypeDefinitions() };

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        return Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("MapOpenApi_SecuredSpec_AddsSecurityStereotypes_ShouldMatchSnapshot");
    }

    [Fact]
    public Task MapOpenApi_AllOfSchemas_HandlesInheritance_ShouldMatchSnapshot()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("all-of-named-enums-security.json");
        var config = ImportConfigurations.CQRSMode();
        var packages = new[] { PackageModels.WithTypeDefinitions() };

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        return Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("MapOpenApi_AllOfSchemas_HandlesInheritance_ShouldMatchSnapshot");
    }

    [Fact]
    public Task MapOpenApi_CleanArchTests_CompleteStructure_ShouldMatchSnapshot()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("clean-arch-tests.json");
        var config = ImportConfigurations.CQRSMode();
        var packages = new[] { PackageModels.WithTypeDefinitions() };

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        return Verify(result)
            .UseDirectory("Snapshots")
            .UseMethodName("MapOpenApi_CleanArchTests_CompleteStructure_ShouldMatchSnapshot");
    }
}
