using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.Modules.Rdbms.Importer.Tests.TestData;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Shouldly;
using VerifyXunit;
using MapperConstants = Intent.Modules.Rdbms.Importer.Tasks.Mappers.Constants;
using static VerifyXunit.Verifier;

namespace Intent.Modules.Rdbms.Importer.Tests;

/// <summary>
/// Comprehensive test suite for database schema to Intent element mapping.
/// Tests all database object types and their properties.
/// </summary>
public class DbSchemaComprehensiveMappingTests
{
    #region Table Mapping Tests

    [Fact]
    public async Task MapTable_BasicProperties_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.SimpleCustomers()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        package.Classes.ShouldNotBeEmpty();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("table-basic");
    }

    [Fact]
    public async Task MapTable_MultipleSchemas_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = 
            [
                Tables.SimpleCustomers(), // dbo schema
                Tables.CustomerInSchema2() // schema2
            ],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("table-multiple-schemas");
    }

    #endregion

    #region Column Mapping Tests

    [Fact]
    public async Task MapColumns_VariousDataTypes_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = 
            [
                Tables.SimpleUsers(), // Int, NVarChar
                Tables.Product() // Int, NVarChar, Decimal with precision/scale
            ],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("columns-data-types");
    }

    [Fact]
    public async Task MapColumns_NullableAndNonNullable_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.SimpleUsers()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("columns-nullable");
    }

    #endregion

    #region Primary Key Tests

    [Fact]
    public async Task MapPrimaryKey_SimplePK_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.SimpleCustomers()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("pk-simple");
    }

    [Fact]
    public async Task MapPrimaryKey_WithIdentityColumn_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.SimpleCustomers()], // Has Id column with IsIdentity = true
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Verify the complete mapping including Primary Key stereotype with Data Source property
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("pk-identity");
    }

    #endregion

    #region Foreign Key Tests

    [Fact]
    public async Task MapForeignKey_SimpleFk_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = 
            [
                Tables.SimpleCustomers(),
                Tables.OrdersWithCustomerFk()
            ],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        package.Associations.ShouldNotBeEmpty();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("fk-simple");
    }

    #endregion

    #region Index Tests

    [Fact]
    public async Task MapIndexes_VariousTypes_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.TableWithIndexesAndTriggers()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("indexes-various");
    }

    #endregion

    #region Constraint Tests

    [Fact]
    public async Task MapConstraints_TextConstraints_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.TableWithConstraints()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("constraints-text");
    }

    [Fact]
    public async Task MapConstraints_DecimalConstraints_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.TableWithConstraints()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("constraints-decimal");
    }

    [Fact]
    public async Task MapConstraints_DefaultConstraints_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.TableWithConstraints()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("constraints-default");
    }

    [Fact]
    public async Task MapConstraints_ComputedColumns_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.TableWithConstraints()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("constraints-computed");
    }

    #endregion

    #region Trigger Tests

    [Fact]
    public async Task MapTriggers_TableTriggers_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.TableWithIndexesAndTriggers()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("triggers-table");
    }

    #endregion

    #region View Tests

    [Fact]
    public async Task MapView_BasicView_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [Views.SimpleCustomerView()],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("view-basic");
    }

    [Fact]
    public async Task MapView_ComplexView_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [Views.CustomerOrdersSummaryView()],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("view-complex");
    }

    [Fact]
    public async Task MapView_ViewWithTrigger_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [Views.ViewWithTrigger()],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("view-with-trigger");
    }

    #endregion

    #region Stored Procedure Tests

    [Fact]
    public async Task MapStoredProcedure_WithInputParameter_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.GetCustomerById()]
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("sp-input-param");
    }

    [Fact]
    public async Task MapStoredProcedure_WithOutputParameter_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.CreateCustomer()]
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("sp-output-param");
    }

    [Fact]
    public async Task MapStoredProcedure_WithInOutParameter_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.UpdateCustomerBalance()]
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("sp-inout-param");
    }

    [Fact]
    public async Task MapStoredProcedure_WithResultSet_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.GetCustomerById()]
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("sp-result-set");
    }

    [Fact]
    public async Task MapStoredProcedure_ComplexWithMultipleFeatures_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.GetOrdersSummary()]
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("sp-complex");
    }

    #endregion

    #region Comprehensive Integration Tests

    [Fact]
    public async Task MapComprehensiveSchema_AllFeatures_ShouldMatchSnapshot()
    {
        // Arrange - Schema with tables, views, stored procedures, all constraint types
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = 
            [
                Tables.SimpleCustomers(),
                Tables.OrdersWithCustomerFk(),
                Tables.TableWithConstraints(),
                Tables.TableWithIndexesAndTriggers()
            ],
            Views = 
            [
                Views.SimpleCustomerView(),
                Views.CustomerOrdersSummaryView()
            ],
            StoredProcedures = 
            [
                StoredProcedures.GetCustomerById(),
                StoredProcedures.CreateCustomer(),
                StoredProcedures.GetOrdersSummary()
            ]
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        package.Classes.ShouldNotBeEmpty();
        package.Associations.ShouldNotBeEmpty();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("comprehensive-all-features");
    }

    [Fact]
    public async Task MapMultiSchemaComprehensive_AllFeatures_ShouldMatchSnapshot()
    {
        // Arrange - Multiple schemas with various objects
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = 
            [
                Tables.SimpleCustomers(), // dbo
                Tables.CustomerInSchema2(), // schema2
                Tables.TableWithConstraints() // dbo
            ],
            Views = 
            [
                Views.SimpleCustomerView(), // dbo
                Views.ViewInSchema2() // schema2
            ],
            StoredProcedures = 
            [
                StoredProcedures.GetCustomerById(), // dbo
                StoredProcedures.ProcedureInSchema2() // schema2
            ]
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("multi-schema-comprehensive");
    }

    #endregion

    #region Composite Keys and Complex Scenarios

    [Fact]
    public async Task MapCompositePrimaryKey_ParentsTable_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [Tables.ParentsWithCompositePK()],
            Views = [],
            StoredProcedures = []
        };
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("composite-pk");
    }

    [Fact]
    public async Task MapCompositeForeignKey_ChildrenToParents_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = DatabaseSchemas.WithParentsAndChildrenCompositePK();
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("composite-fk");
    }

    [Fact]
    public async Task MapAspNetIdentitySchema_AllTables_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = DatabaseSchemas.WithAspNetIdentityTables();
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("aspnet-identity");
    }

    [Fact]
    public async Task MapUniqueIndexConstraint_OrdersWithRefNo_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = DatabaseSchemas.WithOrdersAndUniqueIndex();
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("unique-index");
    }

    #endregion
    
    #region Helper Methods

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

    #endregion

    #region Duplicate Foreign Key Mapping Tests

    [Fact]
    public async Task MapDuplicateFKs_MixedAutoGeneratedAndNamed_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = DatabaseSchemas.WithItemDuplicateFKs();
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Warnings.ShouldContain(w => w.Contains("Duplicate foreign key detected"));
        
        // Should have exactly one association
        package.Associations.ShouldHaveSingleItem();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("duplicate-fks-mixed");
    }

    [Fact]
    public async Task MapDuplicateFKs_BothAutoGenerated_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = DatabaseSchemas.WithItemDuplicateAutoGeneratedFKs();
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Warnings.ShouldContain(w => w.Contains("Duplicate foreign key detected"));
        
        // Should have exactly one association
        package.Associations.ShouldHaveSingleItem();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("duplicate-fks-auto-generated");
    }

    [Fact]
    public async Task MapDuplicateFKs_BothNamed_ShouldMatchSnapshot()
    {
        // Arrange
        var schema = DatabaseSchemas.WithItemDuplicateNamedFKs();
        var package = PackageModels.Empty();
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        result.Warnings.ShouldContain(w => w.Contains("Duplicate foreign key detected"));
        
        // Should have exactly one association
        package.Associations.ShouldHaveSingleItem();
        
        var snapshot = BuildPackageSnapshot(package);
        await Verify(snapshot).UseParameters("duplicate-fks-both-named");
    }

    #endregion
}
