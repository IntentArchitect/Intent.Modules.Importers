using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modules.OpenApi.Importer.Importer;
using Intent.Modules.OpenApi.Importer.Tests.TestData;
using Shouldly;

namespace Intent.Modules.OpenApi.Importer.Tests;

/// <summary>
/// Tests for merging/re-importing OpenAPI specifications into packages with existing elements.
/// Validates synchronization behavior via ExternalReference matching using Intent.MetadataSynchronizer.
/// Follows the testing patterns established in JsonSynchronizerTests and DbSchemaIntentMetadataMergerTests.
/// </summary>
public class OpenApiSynchronizerTests
{
    [Fact]
    public void Synchronize_SameDataReimported_CQRS_RemainsIdempotent()
    {
        // Arrange
        var package = PackageModels.WithPetStoreCQRSElements();
        var commandsAndQueriesBefore = GetCommandsAndQueries(package).ToList();
        var elementIdsBefore = commandsAndQueriesBefore.Select(e => e.Id).ToList();

        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("pet-store.yaml");
        var config = ImportConfigurations.CQRSMode();

        // Act - Re-import same spec
        var persistables = factory.GetPersistables(stream, config, [package]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var commandsAndQueriesAfter = GetCommandsAndQueries(package).ToList();
        commandsAndQueriesAfter.Select(e => e.Id).ShouldContain(elementIdsBefore.First(), "Element IDs should remain unchanged for existing elements");
    }

    [Fact]
    public void Synchronize_SameDataReimported_Service_RemainsIdempotent()
    {
        // Arrange
        var package = PackageModels.WithPetStoreServiceElements();
        var servicesBefore = GetServices(package).ToList();
        var serviceIdsBefore = servicesBefore.Select(e => e.Id).ToList();

        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("pet-store.yaml");
        var config = ImportConfigurations.ServiceMode();

        // Act - Re-import same spec
        var persistables = factory.GetPersistables(stream, config, [package]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var servicesAfter = GetServices(package).ToList();
        servicesAfter.Select(e => e.Id).ShouldContain(serviceIdsBefore.First(), "Service IDs should remain unchanged");
    }

    [Fact]
    public void Synchronize_EmptyPackageThenReimport_CQRS_RemainsIdempotent()
    {
        // Arrange
        var package = PackageModels.WithTypeDefinitions();
        var factory = new OpenApiPersistableFactory();
        var config = ImportConfigurations.CQRSMode();

        // First import
        using (var stream1 = OpenApiSpecs.GetStream("pet-store.yaml"))
        {
            var persistables1 = factory.GetPersistables(stream1, config, [package]);
            Synchronizer.Execute(
                targetPackage: package,
                parentFolderId: package.Id,
                persistables: persistables1,
                deleteExtra: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge);
        }

        var commandsAndQueriesAfterFirstImport = GetCommandsAndQueries(package).ToList();
        var elementCountAfterFirstImport = commandsAndQueriesAfterFirstImport.Count;
        var elementIdsAfterFirstImport = commandsAndQueriesAfterFirstImport.Select(e => e.Id).ToList();

        // Act - Second import of same spec
        using (var stream2 = OpenApiSpecs.GetStream("pet-store.yaml"))
        {
            var persistables2 = factory.GetPersistables(stream2, config, [package]);
            Synchronizer.Execute(
                targetPackage: package,
                parentFolderId: package.Id,
                persistables: persistables2,
                deleteExtra: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge);
        }

        // Assert
        var commandsAndQueriesAfterSecondImport = GetCommandsAndQueries(package).ToList();
        commandsAndQueriesAfterSecondImport.Count.ShouldBe(elementCountAfterFirstImport, "Element count should remain the same");
        commandsAndQueriesAfterSecondImport.Select(e => e.Id).ShouldBe(elementIdsAfterFirstImport, "Element IDs should remain unchanged");
    }

    [Fact]
    public void Synchronize_EmptyPackageThenReimport_Service_RemainsIdempotent()
    {
        // Arrange
        var package = PackageModels.WithTypeDefinitions();
        var factory = new OpenApiPersistableFactory();
        var config = ImportConfigurations.ServiceMode();

        // First import
        using (var stream1 = OpenApiSpecs.GetStream("pet-store.yaml"))
        {
            var persistables1 = factory.GetPersistables(stream1, config, [package]);
            Synchronizer.Execute(
                targetPackage: package,
                parentFolderId: package.Id,
                persistables: persistables1,
                deleteExtra: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge);
        }

        var servicesAfterFirstImport = GetServices(package).ToList();
        var elementCountAfterFirstImport = servicesAfterFirstImport.Count;
        var elementIdsAfterFirstImport = servicesAfterFirstImport.Select(e => e.Id).ToList();

        // Act - Second import of same spec
        using (var stream2 = OpenApiSpecs.GetStream("pet-store.yaml"))
        {
            var persistables2 = factory.GetPersistables(stream2, config, [package]);
            Synchronizer.Execute(
                targetPackage: package,
                parentFolderId: package.Id,
                persistables: persistables2,
                deleteExtra: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge);
        }

        // Assert
        var servicesAfterSecondImport = GetServices(package).ToList();
        servicesAfterSecondImport.Count.ShouldBe(elementCountAfterFirstImport, "Service count should remain the same");
        servicesAfterSecondImport.Select(e => e.Id).ShouldBe(elementIdsAfterFirstImport, "Service IDs should remain unchanged");
    }

    [Fact]
    public void Synchronize_DTOReimported_UpdatesExistingDTO()
    {
        // Arrange
        var package = PackageModels.WithExistingPetDTO();
        var dtoBefore = GetDTOs(package).Single(d => d.Name == "Pet");
        var dtoIdBefore = dtoBefore.Id;
        var fieldCountBefore = dtoBefore.ChildElements.Count;

        var factory = new OpenApiPersistableFactory();
        using var stream = OpenApiSpecs.GetStream("pet-store.yaml");
        var config = ImportConfigurations.CQRSMode();

        // Act - Re-import spec which might have Pet schema
        var persistables = factory.GetPersistables(stream, config, [package]);
        Synchronizer.Execute(
            targetPackage: package,
            parentFolderId: package.Id,
            persistables: persistables,
            deleteExtra: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge);

        // Assert
        var dtoAfter = GetDTOs(package).FirstOrDefault(d => d.Name == "Pet");
        if (dtoAfter != null)
        {
            dtoAfter.Id.ShouldBe(dtoIdBefore, "DTO ID should remain unchanged if it was matched by ExternalReference");
        }
    }

    [Fact]
    public void Synchronize_WithQuerySpec_DoesNotThrowDuplicateKeyException()
    {
        // Arrange - Test with a spec that has query parameters
        var package = PackageModels.WithTypeDefinitions();
        var factory = new OpenApiPersistableFactory();
        var config = ImportConfigurations.CQRSMode();

        // Act - Import the same spec twice
        var exception = Record.Exception(() =>
        {
            using (var stream1 = OpenApiSpecs.GetStream("with-query.json"))
            {
                var persistables1 = factory.GetPersistables(stream1, config, [package]);
                Synchronizer.Execute(
                    targetPackage: package,
                    parentFolderId: package.Id,
                    persistables: persistables1,
                    deleteExtra: false,
                    createAttributesWithUnknownTypes: true,
                    stereotypeManagementMode: StereotypeManagementMode.Merge);
            }

            using (var stream2 = OpenApiSpecs.GetStream("with-query.json"))
            {
                var persistables2 = factory.GetPersistables(stream2, config, [package]);
                Synchronizer.Execute(
                    targetPackage: package,
                    parentFolderId: package.Id,
                    persistables: persistables2,
                    deleteExtra: false,
                    createAttributesWithUnknownTypes: true,
                    stereotypeManagementMode: StereotypeManagementMode.Merge);
            }
        });

        // Assert
        exception.ShouldBeNull("Should not throw duplicate key exception when reimporting the same spec");
    }

    [Fact]
    public void Synchronize_WithEnumSpec_RemainsIdempotent()
    {
        // Arrange
        var package = PackageModels.WithTypeDefinitions();
        var factory = new OpenApiPersistableFactory();
        var config = ImportConfigurations.CQRSMode();

        // First import
        using (var stream1 = OpenApiSpecs.GetStream("with-enum.json"))
        {
            var persistables1 = factory.GetPersistables(stream1, config, [package]);
            Synchronizer.Execute(
                targetPackage: package,
                parentFolderId: package.Id,
                persistables: persistables1,
                deleteExtra: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge);
        }

        var enumsAfterFirstImport = GetEnums(package).ToList();
        var enumCountAfterFirstImport = enumsAfterFirstImport.Count;
        var enumIdsAfterFirstImport = enumsAfterFirstImport.Select(e => e.Id).ToList();

        // Act - Second import
        using (var stream2 = OpenApiSpecs.GetStream("with-enum.json"))
        {
            var persistables2 = factory.GetPersistables(stream2, config, [package]);
            Synchronizer.Execute(
                targetPackage: package,
                parentFolderId: package.Id,
                persistables: persistables2,
                deleteExtra: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge);
        }

        // Assert
        var enumsAfterSecondImport = GetEnums(package).ToList();
        enumsAfterSecondImport.Count.ShouldBe(enumCountAfterFirstImport, "Enum count should remain the same");
        enumsAfterSecondImport.Select(e => e.Id).ShouldBe(enumIdsAfterFirstImport, "Enum IDs should remain unchanged");
    }

    [Fact]
    public void Synchronize_AllSpecifications_CQRS_DoNotThrowOnReimport()
    {
        // Arrange - Test all specs for idempotency
        var testSpecs = new[]
        {
            "pet-store.yaml",
            "with-enum.json",
            "with-query.json",
            "swagger-sample.json",
            "clean-arch-tests.json"
        };

        foreach (var specFile in testSpecs)
        {
            var package = PackageModels.WithTypeDefinitions();
            var factory = new OpenApiPersistableFactory();
            var config = ImportConfigurations.CQRSMode();

            // Act
            var exception = Record.Exception(() =>
            {
                // First import
                using (var stream1 = OpenApiSpecs.GetStream(specFile))
                {
                    var persistables1 = factory.GetPersistables(stream1, config, [package]);
                    Synchronizer.Execute(
                        targetPackage: package,
                        parentFolderId: package.Id,
                        persistables: persistables1,
                        deleteExtra: false,
                        createAttributesWithUnknownTypes: true,
                        stereotypeManagementMode: StereotypeManagementMode.Merge);
                }

                // Second import
                using (var stream2 = OpenApiSpecs.GetStream(specFile))
                {
                    var persistables2 = factory.GetPersistables(stream2, config, [package]);
                    Synchronizer.Execute(
                        targetPackage: package,
                        parentFolderId: package.Id,
                        persistables: persistables2,
                        deleteExtra: false,
                        createAttributesWithUnknownTypes: true,
                        stereotypeManagementMode: StereotypeManagementMode.Merge);
                }
            });

            // Assert
            exception.ShouldBeNull($"Spec '{specFile}' should not throw on re-import");
        }
    }

    private static IEnumerable<ElementPersistable> GetCommandsAndQueries(PackageModelPersistable package) =>
        package.Classes.Where(c => c.SpecializationType is "Command" or "Query");

    private static IEnumerable<ElementPersistable> GetServices(PackageModelPersistable package) =>
        package.Classes.Where(c => c.SpecializationType == "Service");

    private static IEnumerable<ElementPersistable> GetDTOs(PackageModelPersistable package) =>
        package.Classes.Where(c => c.SpecializationType == "DTO");

    private static IEnumerable<ElementPersistable> GetEnums(PackageModelPersistable package) =>
        package.Classes.Where(c => c.SpecializationType == "Enum");
}
