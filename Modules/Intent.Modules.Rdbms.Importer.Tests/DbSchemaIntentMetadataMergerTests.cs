using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.Modules.Rdbms.Importer.Tests.TestData;
using Shouldly;

namespace Intent.Modules.Rdbms.Importer.Tests;

public class DbSchemaIntentMetadataMergerTests
{
    [Fact]
    public void MergeSchemaAndPackage_TableWithColumns_CreatesClassElementWithAttributes()
    {
        // Arrange
        var scenario = ScenarioComposer.SchemaOnly(DatabaseSchemas.WithSimpleUsersTable());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var userClass = GetClasses(scenario.Package).ShouldHaveSingleItem();
        userClass.Name.ShouldBe("User");
        GetAttributeNames(userClass).ShouldBe(new[] { "Id", "Name", "Email" });
    }

    [Fact]
    public void MergeSchemaAndPackage_CustomerAndOrderTablesToEmptyPackage_CreatesClassesAndAssociation()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomersAndOrders(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var classes = GetClasses(scenario.Package).ToList();
        classes.Count.ShouldBe(2);
        classes.ShouldContain(c => c.Name == "Customer");
        classes.ShouldContain(c => c.Name == "Order");
        scenario.Package.Associations.ShouldHaveSingleItem();
    }

    [Fact]
    public void MergeSchemaAndPackage_SameDataReimported_RemainsIdempotent()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomersAndOrders(), PackageModels.WithCustomerAndOrderTables());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());
        var classIdsBefore = GetClasses(scenario.Package).Select(c => c.Id).ToList();
        var associationIdsBefore = scenario.Package.Associations.Select(a => a.Id).ToList();

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var classes = GetClasses(scenario.Package).ToList();
        classes.Count.ShouldBe(2);
        classes.Select(c => c.Id).ShouldBe(classIdsBefore);
        scenario.Package.Associations.ShouldHaveSingleItem();
        scenario.Package.Associations.Select(a => a.Id).ShouldBe(associationIdsBefore);
    }

    [Fact]
    public void MergeSchemaAndPackage_NewTableDetected_AddsOnlyNewTable()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomerAndNewProduct(), PackageModels.WithCustomerTable());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var classes = GetClasses(scenario.Package).ToList();
        classes.Count.ShouldBe(2);
        classes.ShouldContain(c => c.Name == "Customer");
        classes.ShouldContain(c => c.Name == "Product");
    }

    [Fact]
    public void MergeSchemaAndPackage_ExistingTableWithNewColumn_AddsNewAttribute()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomerWithNewAddressColumn(), PackageModels.WithCustomerTable());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var customer = GetClasses(scenario.Package).ShouldHaveSingleItem();
        GetAttributeNames(customer).ShouldBe(new[] { "Id", "Email", "Address" });
    }

    [Fact]
    public void MergeSchemaAndPackage_ColumnRemovedWithAllowDeletionsFalse_KeepsAttribute()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomerMissingEmailColumn(), PackageModels.WithCustomerTable());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var customer = GetClasses(scenario.Package).ShouldHaveSingleItem();
        GetAttributeNames(customer).ShouldBe(new[] { "Id", "Email" });
    }

    [Fact]
    public void MergeSchemaAndPackage_ColumnRemovedWithAllowDeletionsTrue_RemovesAttribute()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomerMissingEmailColumn(), PackageModels.WithCustomerTable());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesWithDeletions());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var customer = GetClasses(scenario.Package).ShouldHaveSingleItem();
        GetAttributeNames(customer).ShouldBe(new[] { "Id" });
    }

    [Fact]
    public void MergeSchemaAndPackage_ForeignKeyRemovedWithAllowDeletionsTrue_RemovesAssociation()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomerAndOrderWithoutForeignKey(), PackageModels.WithCustomerAndOrderTables());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesWithDeletions());
        var associationCountBefore = scenario.Package.Associations.Count;

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        associationCountBefore.ShouldBe(1, "Should have started with one association");
        scenario.Package.Associations.ShouldBeEmpty("Association should have been removed when FK no longer exists");
    }

    [Fact]
    public void MergeSchemaAndPackage_NewTableWithForeignKeyToExistingTable_CreatesClassAndAssociation()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithOrderAndExistingCustomerReference(), PackageModels.WithExistingCustomer());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var classes = GetClasses(scenario.Package).ToList();
        classes.Count.ShouldBe(2);
        classes.ShouldContain(c => c.Name == "Customer");
        classes.ShouldContain(c => c.Name == "Order");
        scenario.Package.Associations.ShouldHaveSingleItem();
        var association = scenario.Package.Associations.Single();
        GetClassById(classes, association.SourceEnd.TypeReference.TypeId).Name.ShouldBe("Order");
        GetClassById(classes, association.TargetEnd.TypeReference.TypeId).Name.ShouldBe("Customer");
    }

    [Fact]
    public void MergeSchemaAndPackage_MultipleSchemas_ImportsTablesFromBothSchemas()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithMultipleSchemas(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var classes = GetClasses(scenario.Package).ToList();
        classes.Count.ShouldBe(2);
        classes.ShouldContain(c => string.Equals(c.ExternalReference, "[dbo].[customers]", StringComparison.OrdinalIgnoreCase));
        classes.ShouldContain(c => string.Equals(c.ExternalReference, "[schema2].[customer]", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void MergeSchemaAndPackage_AttributeWithCustomType_PreservesCustomTypeWhenConfigured()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(
            DatabaseSchemas.WithSimpleUsersTableWithStatus(),
            PackageModels.WithUserHavingCustomEnumType());
        var configWithPreservation = ImportConfigurations.TablesOnly();
        configWithPreservation.PreserveAttributeTypes = true;
        var merger = new DbSchemaIntentMetadataMerger(configWithPreservation);

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var userClass = GetClasses(scenario.Package).ShouldHaveSingleItem();
        var statusAttribute = userClass.ChildElements.Single(a => a.Name == "Status");
        statusAttribute.TypeReference.TypeId.ShouldBe("custom-enum-id", "Custom enum type should be preserved");
    }

    [Fact]
    public void MergeSchemaAndPackage_AttributeWithCustomType_OverridesCustomTypeWhenNotConfigured()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(
            DatabaseSchemas.WithSimpleUsersTableWithStatus(),
            PackageModels.WithUserHavingCustomEnumType());
        var configWithoutPreservation = ImportConfigurations.TablesOnly();
        configWithoutPreservation.PreserveAttributeTypes = false;
        var merger = new DbSchemaIntentMetadataMerger(configWithoutPreservation);

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var userClass = GetClasses(scenario.Package).ShouldHaveSingleItem();
        var statusAttribute = userClass.ChildElements.Single(a => a.Name == "Status");
        statusAttribute.TypeReference.TypeId.ShouldNotBe("custom-enum-id", "Custom enum type should be overridden with database type");
    }

    [Fact]
    public void MergeSchemaAndPackage_TableWithCompositePK_CreatesBothPrimaryKeyStereotypes()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithParentsAndChildrenCompositePK(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var parentClass = GetClasses(scenario.Package).Single(c => c.Name == "Parent");
        var pkAttributes = parentClass.ChildElements
            .Where(a => a.Stereotypes.Any(s => s.Name == "Primary Key"))
            .ToList();
        pkAttributes.Count.ShouldBe(2);
        pkAttributes.ShouldContain(a => a.Name == "Id");
        pkAttributes.ShouldContain(a => a.Name == "Id2");
    }

    [Fact]
    public void MergeSchemaAndPackage_TableWithCompositeFk_CreatesAssociationWithMultipleFkAttributes()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithParentsAndChildrenCompositePK(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        scenario.Package.Associations.ShouldHaveSingleItem();
        var childClass = GetClasses(scenario.Package).Single(c => c.Name == "Child");
        var fkAttributes = childClass.ChildElements
            .Where(a => a.Stereotypes.Any(s => s.Name == "Foreign Key"))
            .ToList();
        fkAttributes.Count.ShouldBe(2);
        fkAttributes.ShouldContain(a => a.Name == "ParentId");
        fkAttributes.ShouldContain(a => a.Name == "ParentId2");
    }

    [Fact]
    public void MergeSchemaAndPackage_TableWithMultipleForeignKeys_CreatesAllAssociations()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithTableWithMultipleFKs(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        scenario.Package.Associations.Count.ShouldBe(5);
        var primaryTable = GetClasses(scenario.Package).Single(c => c.Name == "PrimaryTable");
        var fkAttributes = primaryTable.ChildElements
            .Where(a => a.Stereotypes.Any(s => s.Name == "Foreign Key"))
            .ToList();
        fkAttributes.Count.ShouldBe(5);
        fkAttributes.Select(a => a.Name).ShouldBe(new[] { "FKTableId1", "FKTableId2", "FKAsTableId3", "FKTryTableId4", "FKThisTableId5" });
    }

    [Fact]
    public void MergeSchemaAndPackage_SelfReferencingTable_CreatesCircularAssociation()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithSelfReferencingTable(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var selfRefTable = GetClasses(scenario.Package).ShouldHaveSingleItem();
        selfRefTable.Name.ShouldBe("SelfReferenceTable");
        scenario.Package.Associations.ShouldHaveSingleItem();
        var association = scenario.Package.Associations.Single();
        association.SourceEnd.TypeReference.TypeId.ShouldBe(selfRefTable.Id);
        association.TargetEnd.TypeReference.TypeId.ShouldBe(selfRefTable.Id);
    }

    [Fact]
    public void MergeSchemaAndPackage_LegacyTableWithoutPK_DoesNotCreatePrimaryKeyStereotypes()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithLegacyTableNoPK(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var legacyTable = GetClasses(scenario.Package).ShouldHaveSingleItem();
        legacyTable.Name.ShouldBe("LegacyTable");
        var pkAttributes = legacyTable.ChildElements
            .Where(a => a.Stereotypes.Any(s => s.Name == "Primary Key"))
            .ToList();
        pkAttributes.ShouldBeEmpty();
    }

    [Fact]
    public void MergeSchemaAndPackage_LegacyTableWithUnderscoreName_MapsNameCorrectly()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithLegacyTableNoPK(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        var legacyTable = GetClasses(scenario.Package).ShouldHaveSingleItem();
        legacyTable.Name.ShouldBe("LegacyTable");
        legacyTable.ExternalReference.ShouldBe("[dbo].[legacy_table]");
        GetAttributeNames(legacyTable).ShouldContain("LegacyId");
    }

    [Fact]
    public void MergeSchemaAndPackage_InclusiveImportTableBOnly_ShouldPreserveAssociationFromTableAAndRemoveAssociationFromTableB()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(
            DatabaseSchemas.WithOnlyTableB(), // Only Table B is being imported (inclusive import simulation)
            PackageModels.WithTableABCAndBothAssociations()); // Package has A→B and B→C associations
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesWithDeletions());
        
        var classes = GetClasses(scenario.Package).ToList();
        var tableA = classes.Single(c => c.Name == "TableA");
        var tableB = classes.Single(c => c.Name == "TableB");
        var tableC = classes.Single(c => c.Name == "TableC");
        
        // Capture initial associations
        var associationAtoB = scenario.Package.Associations.Single(a => 
            a.SourceEnd.TypeReference.TypeId == tableA.Id && 
            a.TargetEnd.TypeReference.TypeId == tableB.Id);
        var associationBtoC = scenario.Package.Associations.Single(a => 
            a.SourceEnd.TypeReference.TypeId == tableB.Id && 
            a.TargetEnd.TypeReference.TypeId == tableC.Id);

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert - This test currently fails, demonstrating the bug
        result.IsSuccessful.ShouldBeTrue();
        
        // EXPECTED BEHAVIOR:
        // - A→B association should remain (FK still exists in real database, just not imported)
        // - B→C association should be removed (FK no longer exists in database)
        scenario.Package.Associations.Count.ShouldBe(1, "Only the A→B association should remain");
        
        scenario.Package.Associations.ShouldContain(a => a.Id == associationAtoB.Id, 
            "A→B association should be preserved (FK exists in database, table just wasn't imported)");
        scenario.Package.Associations.ShouldNotContain(a => a.Id == associationBtoC.Id, 
            "B→C association should be removed (FK no longer exists in database)");
    }

    private static IEnumerable<ElementPersistable> GetClasses(PackageModelPersistable package) =>
        package.Classes.Where(c =>
            string.Equals(c.SpecializationType, ClassModel.SpecializationType, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<string> GetAttributeNames(ElementPersistable element) =>
        element.ChildElements.Where(p => p.SpecializationTypeId == AttributeModel.SpecializationTypeId)
            .Select(a => a.Name);

    private static ElementPersistable GetClassById(IEnumerable<ElementPersistable> classes, string id) =>
        classes.Single(c => c.Id == id);

}