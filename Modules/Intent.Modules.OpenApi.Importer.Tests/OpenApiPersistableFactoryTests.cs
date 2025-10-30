using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.OpenApi.Importer.Importer;
using Intent.Modules.OpenApi.Importer.Tests.TestData;
using Shouldly;

namespace Intent.Modules.OpenApi.Importer.Tests;

/// <summary>
/// Behavioral tests for OpenApiPersistableFactory following RDBMS test guide conventions.
/// Tests use Object Mother factories and focus on state-based assertions (not message-based).
/// </summary>
public class OpenApiPersistableFactoryTests
{
    [Theory]
    [MemberData(nameof(GetAllSpecifications))]
    public void GetPersistables_AllSpecs_CQRS_ShouldNotThrow(string specName, string fileName)
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream(fileName);
        var config = ImportConfigurations.CQRSMode();
        PackageModelPersistable[] packages = [PackageModels.WithTypeDefinitions()];

        // Act
        var exception = Record.Exception(() =>
        {
            factory.GetPersistables(stream, config, packages);
        });

        // Assert
        exception.ShouldBeNull($"CQRS mode should not throw for {specName}");
    }

    [Theory]
    [MemberData(nameof(GetAllSpecifications))]
    public void GetPersistables_AllSpecs_Service_ShouldNotThrow(string specName, string fileName)
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream(fileName);
        var config = ImportConfigurations.ServiceMode();
        PackageModelPersistable[] packages = [PackageModels.WithTypeDefinitions()];

        // Act
        var exception = Record.Exception(() =>
        {
            factory.GetPersistables(stream, config, packages);
        });

        // Assert
        exception.ShouldBeNull($"Service mode should not throw for {specName}");
    }

    [Fact]
    public void GetPersistables_PetStore_CQRS_CreatesCommandsAndQueries()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("pet-store.yaml");
        var config = ImportConfigurations.CQRSMode();
        var package = PackageModels.WithTypeDefinitions();
        PackageModelPersistable[] packages = [package];

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        result.Elements.ShouldContain(c => c.SpecializationType == "Command", "Should create Command elements for POST/PUT/DELETE operations");
        result.Elements.ShouldContain(c => c.SpecializationType == "Query", "Should create Query elements for GET operations");
    }

    [Fact]
    public void GetPersistables_PetStore_Service_CreatesServicesAndOperations()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("pet-store.yaml");
        var config = ImportConfigurations.ServiceMode();
        var package = PackageModels.WithTypeDefinitions();
        PackageModelPersistable[] packages = [package];

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        result.Elements.ShouldContain(c => c.SpecializationType == "Service", "Should create Service elements");
        var services = result.Elements.Where(c => c.SpecializationType == "Service").ToList();
        services.ShouldNotBeEmpty("Should have at least one service");
    }

    [Fact]
    public void GetPersistables_WithEnum_CreatesEnumElements()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("with-enum.json");
        var config = ImportConfigurations.CQRSMode();
        var package = PackageModels.WithTypeDefinitions();
        PackageModelPersistable[] packages = [package];

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        result.Elements.ShouldContain(c => c.SpecializationType == "Enum", "Should create Enum elements for enum schemas");
    }

    [Fact]
    public void GetPersistables_WithQuery_MapsQueryParameters()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("with-query.json");
        var config = ImportConfigurations.CQRSMode();
        var package = PackageModels.WithTypeDefinitions();
        PackageModelPersistable[] packages = [package];

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        var queries = result.Elements.Where(c => c.SpecializationType == "Query").ToList();
        queries.ShouldNotBeEmpty("Should create Query elements");
    }

    [Fact]
    public void GetPersistables_IntentGeneratedSecured_AddsSecurityStereotypes()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("intent-generated-secured.json");
        var config = ImportConfigurations.CQRSMode();
        var package = PackageModels.WithTypeDefinitions();
        PackageModelPersistable[] packages = [package];

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        var securedElements = result.Elements
            .Where(c => c.SpecializationType is "Command" or "Query")
            .Where(c => c.Stereotypes.Any())
            .ToList();
        
        securedElements.ShouldNotBeEmpty("Secured operations should have stereotypes applied");
    }

    [Fact]
    public void GetPersistables_SwaggerSample_CreatesDTOElements()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("swagger-sample.json");
        var config = ImportConfigurations.CQRSMode();
        var package = PackageModels.WithTypeDefinitions();
        PackageModelPersistable[] packages = [package];

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        result.Elements.ShouldContain(c => c.SpecializationType == "DTO", "Should create DTO elements for request/response schemas");
    }

    [Fact]
    public void GetPersistables_AllOfNamedEnumsSecurity_HandlesAllOfSchemas()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("all-of-named-enums-security.json");
        var config = ImportConfigurations.CQRSMode();
        var package = PackageModels.WithTypeDefinitions();
        PackageModelPersistable[] packages = [package];

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        result.Elements.ShouldNotBeEmpty("Should generate elements from AllOf schemas");
    }

    [Fact]
    public void GetPersistables_AzureOpenApi_HandlesComplexSpec()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("azure-open-api.json");
        var config = ImportConfigurations.CQRSMode();
        var package = PackageModels.WithTypeDefinitions();
        PackageModelPersistable[] packages = [package];

        // Act
        var exception = Record.Exception(() =>
        {
            factory.GetPersistables(stream, config, packages);
        });

        // Assert
        exception.ShouldBeNull("Should handle complex Azure OpenAPI spec without throwing");
    }

    [Fact]
    public void GetPersistables_CleanArchTests_CreatesExpectedStructure()
    {
        // Arrange
        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("clean-arch-tests.json");
        var config = ImportConfigurations.CQRSMode();
        var package = PackageModels.WithTypeDefinitions();
        PackageModelPersistable[] packages = [package];

        // Act
        var result = factory.GetPersistables(stream, config, packages);

        // Assert
        result.Elements.Count.ShouldBeGreaterThan(0, "Should create elements from Clean Architecture spec");
        
        var commandsAndQueries = result.Elements
            .Where(c => c.SpecializationType is "Command" or "Query")
            .ToList();
        
        commandsAndQueries.ShouldNotBeEmpty("Should create Commands and Queries");
    }

    public static IEnumerable<object[]> GetAllSpecifications()
    {
        yield return new object[] { "PetStore", "pet-store.yaml" };
        yield return new object[] { "PetStoreV1", "PetStorevOpenApi1.0.yaml" };
        yield return new object[] { "WithEnum", "with-enum.json" };
        yield return new object[] { "WithQuery", "with-query.json" };
        yield return new object[] { "WithQuery2", "with-query2.json" };
        yield return new object[] { "SwaggerSample", "swagger-sample.json" };
        yield return new object[] { "AllOfNamedEnumsSecurity", "all-of-named-enums-security.json" };
        yield return new object[] { "AzureOpenApi", "azure-open-api.json" };
        yield return new object[] { "CleanArchTests", "clean-arch-tests.json" };
        yield return new object[] { "IntentGeneratedSecured", "intent-generated-secured.json" };
    }
}


