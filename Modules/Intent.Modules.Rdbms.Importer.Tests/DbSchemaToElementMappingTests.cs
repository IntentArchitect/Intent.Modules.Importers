using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.Modules.Rdbms.Importer.Tests.TestData;
using Shouldly;
using VerifyXunit;
using MapperConstants = Intent.Modules.Rdbms.Importer.Tasks.Mappers.Constants;
using static VerifyXunit.Verifier;

namespace Intent.Modules.Rdbms.Importer.Tests;

public class DbSchemaToElementMappingTests
{
    [Fact]
    public async Task MapComprehensiveSchema_ShouldMatchSnapshot()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomersAndOrders(), PackageModels.Empty())
            .WithSchemaMutation(schema =>
            {
                if (schema.Tables.All(t => !string.Equals(t.Name, "Products", StringComparison.OrdinalIgnoreCase)))
                {
                    schema.Tables.Add(Tables.Product());
                }

                var customers = schema.Tables.Single(t => string.Equals(t.Name, "Customers", StringComparison.OrdinalIgnoreCase));
                if (customers.Columns.All(c => !string.Equals(c.Name, "Address", StringComparison.OrdinalIgnoreCase)))
                {
                    customers.Columns.Add(Tables.Column("Address", SqlDbType.NVarChar, length: 500, isNullable: true));
                }
            });
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();

        var snapshot = BuildPackageSnapshot(scenario.Package);
        await Verify(snapshot).UseParameters("comprehensive");
    }

    [Fact]
    public async Task MapExistingPackageWithSchemaChanges_ShouldMatchSnapshot()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomerWithNewAddressColumn(), PackageModels.WithCustomerTable())
            .WithSchemaMutation(schema =>
            {
                var customers = schema.Tables.Single(t => string.Equals(t.Name, "Customers", StringComparison.OrdinalIgnoreCase));
                if (customers.Columns.All(c => !string.Equals(c.Name, "CreatedOn", StringComparison.OrdinalIgnoreCase)))
                {
                    customers.Columns.Add(Tables.Column("CreatedOn", SqlDbType.DateTime2, isNullable: false));
                }
            });
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();

        var snapshot = BuildPackageSnapshot(scenario.Package);
        await Verify(snapshot).UseParameters("existing-package-update");
    }

    [Fact]
    public async Task MapSchemaWithForeignKey_ShouldMatchSnapshot()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithOrderAndExistingCustomerReference(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();

        var snapshot = BuildPackageSnapshot(scenario.Package);
        await Verify(snapshot).UseParameters("foreign-key");
    }

    private static object BuildPackageSnapshot(PackageModelPersistable package)
    {
        var elementNamesById = package.Classes.ToDictionary(c => c.Id, c => c.Name);

        return new
        {
            Package = new
            {
                package.Name,
                Metadata = SnapshotMetadata(package.Metadata),
                Folders = package.Classes
                    .Where(IsFolder)
                    .OrderBy(e => e.Name)
                    .Select(e => SnapshotElement(e, elementNamesById)),
                Classes = package.Classes
                    .Where(IsClass)
                    .OrderBy(e => e.Name)
                    .Select(e => SnapshotElement(e, elementNamesById)),
                Associations = package.Associations
                    .OrderBy(a => a.ExternalReference)
                    .Select(a => SnapshotAssociation(a, elementNamesById))
            }
        };
    }

    private static object SnapshotElement(ElementPersistable element, IReadOnlyDictionary<string, string> elementNamesById)
    {
        var children = element.ChildElements ?? new List<ElementPersistable>();

        return new
        {
            element.Name,
            element.SpecializationType,
            element.ExternalReference,
            element.IsAbstract,
            element.IsMapped,
            TypeReference = SnapshotTypeReference(element.TypeReference, elementNamesById),
            Metadata = SnapshotMetadata(element.Metadata),
            Stereotypes = SnapshotStereotypes(element.Stereotypes),
            Children = children
                .OrderBy(child => child.SpecializationType)
                .ThenBy(child => child.Name)
                .Select(child => SnapshotElement(child, elementNamesById))
        };
    }

    private static object SnapshotAssociation(AssociationPersistable association, IReadOnlyDictionary<string, string> elementNamesById)
    {
        return new
        {
            association.ExternalReference,
            Source = SnapshotAssociationEnd(association.SourceEnd, elementNamesById),
            Target = SnapshotAssociationEnd(association.TargetEnd, elementNamesById)
        };
    }

    private static object SnapshotAssociationEnd(AssociationEndPersistable end, IReadOnlyDictionary<string, string> elementNamesById)
    {
        return new
        {
            end.Name,
            end.ExternalReference,
            TypeReference = SnapshotTypeReference(end.TypeReference, elementNamesById),
            Stereotypes = SnapshotStereotypes(end.Stereotypes)
        };
    }

    private static object? SnapshotTypeReference(TypeReferencePersistable? typeReference, IReadOnlyDictionary<string, string>? elementNamesById)
    {
        if (typeReference == null)
        {
            return null;
        }

        return new
        {
            typeReference.TypeId,
            ResolvedName = typeReference.TypeId != null && elementNamesById != null && elementNamesById.TryGetValue(typeReference.TypeId, out var resolvedName)
                ? resolvedName
                : null,
            typeReference.GenericTypeId,
            typeReference.IsNullable,
            typeReference.IsCollection,
            typeReference.IsNavigable,
            typeReference.IsRequired
        };
    }

    private static IEnumerable<object> SnapshotMetadata(ICollection<GenericMetadataPersistable>? metadata)
    {
        return metadata == null
            ? Array.Empty<object>()
            : metadata
                .OrderBy(m => m.Key)
                .Select(m => new { m.Key, m.Value });
    }

    private static IEnumerable<object> SnapshotStereotypes(ICollection<StereotypePersistable>? stereotypes)
    {
        if (stereotypes == null)
        {
            return Array.Empty<object>();
        }

        return stereotypes
            .OrderBy(s => s.DefinitionId)
            .ThenBy(s => s.Name)
            .Select(s =>
            {
                var properties = s.Properties ?? new List<StereotypePropertyPersistable>();

                return new
                {
                    s.Name,
                    s.DefinitionId,
                    Properties = properties
                        .OrderBy(p => p.DefinitionId)
                        .Select(p => new
                        {
                            p.Name,
                            p.DefinitionId,
                            p.Value,
                            p.IsActive
                        })
                };
            });
    }

    private static bool IsFolder(ElementPersistable element) =>
        string.Equals(element.SpecializationType, MapperConstants.SpecializationTypes.Folder.SpecializationType, StringComparison.OrdinalIgnoreCase);

    private static bool IsClass(ElementPersistable element) =>
        string.Equals(element.SpecializationType, ClassModel.SpecializationType, StringComparison.OrdinalIgnoreCase);
}
