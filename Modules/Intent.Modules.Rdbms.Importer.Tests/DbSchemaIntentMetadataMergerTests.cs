using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.Modules.Rdbms.Importer.Tests.TestData;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
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
        var orderClass = GetClasses(scenario.Package).Single(c => c.Name == "Order");
        var customerIdAttribute = orderClass.ChildElements.Single(a => a.Name == "CustomerId");
        var fkStereotypeCountBefore = customerIdAttribute.Stereotypes.Count(s => s.Name == "Foreign Key");

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        associationCountBefore.ShouldBe(1, "Should have started with one association");
        scenario.Package.Associations.ShouldBeEmpty("Association should have been removed when FK no longer exists");
        fkStereotypeCountBefore.ShouldBe(1, "CustomerId should have started with a FK stereotype");
        var fkStereotypeCountAfter = customerIdAttribute.Stereotypes.Count(s => s.Name == "Foreign Key");
        fkStereotypeCountAfter.ShouldBe(0, "FK stereotype should have been removed from CustomerId attribute when association was removed");
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
        GetClassById(classes, association.SourceEnd.TypeReference.TypeId!).Name.ShouldBe("Order");
        GetClassById(classes, association.TargetEnd.TypeReference.TypeId!).Name.ShouldBe("Customer");
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
    public void MergeSchemaAndPackage_SharedPrimaryKeyForeignKey_DoesNotApplyForeignKeyStereotype()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(DatabaseSchemas.WithCustomerAndRiskProfileSharedPrimaryKey(), PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        scenario.Package.Associations.ShouldHaveSingleItem();
        var riskProfile = GetClasses(scenario.Package).Single(c => c.Name == "RiskProfile");
        var idAttribute = riskProfile.ChildElements.Single(a => a.Name == "Id");
        idAttribute.Stereotypes.ShouldContain(s => s.Name == "Primary Key");
        idAttribute.Stereotypes.ShouldNotContain(s => s.Name == "Foreign Key");
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

    [Fact]
    public void MergeSchemaAndPackage_IndexRemovedWithAllowDeletionsTrue_RemovesIndex()
    {
        // Arrange - Start with table that has an index
        var schemaWithIndex = DatabaseSchemas.WithSimpleUsersTable();
        schemaWithIndex.Tables[0].Indexes =
        [
            Indexes.UniqueEmailIndex()
        ];
        
        var scenario = ScenarioComposer.SchemaOnly(schemaWithIndex);
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesWithDeletions());
        
        // Initial import with index
        merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Verify index was created
        var indexesBefore = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        indexesBefore.Count.ShouldBe(1, "Should have created one index");
        var createdIndex = indexesBefore[0];
        createdIndex.Name.ShouldBe("IX_Customers_Email_Unique");
        createdIndex.ExternalReference.ShouldContain("ix_customers_email_unique");
        
        // Now remove the index from the schema
        schemaWithIndex.Tables[0].Indexes = [];
        
        // Act - Re-import without the index
        var result = merger.MergeSchemaAndPackage(schemaWithIndex, scenario.Package);
        
        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var indexesAfter = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        indexesAfter.ShouldBeEmpty("Index should have been removed when it no longer exists in database");
    }

    [Fact]
    public void MergeSchemaAndPackage_IndexRemovedWithAllowDeletionsFalse_KeepsIndex()
    {
        // Arrange - Start with table that has an index
        var schemaWithIndex = DatabaseSchemas.WithSimpleUsersTable();
        schemaWithIndex.Tables[0].Indexes =
        [
            Indexes.UniqueEmailIndex()
        ];
        
        var scenario = ScenarioComposer.SchemaOnly(schemaWithIndex);
        var mergerWithDeletions = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesWithDeletions());
        
        // Initial import with index
        mergerWithDeletions.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Verify index was created
        var indexesBefore = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        indexesBefore.Count.ShouldBe(1);
        var originalIndexId = indexesBefore[0].Id;
        
        // Now remove the index from the schema but use config without deletions
        schemaWithIndex.Tables[0].Indexes = [];
        var mergerWithoutDeletions = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());
        
        // Act - Re-import without the index but with AllowDeletions=false
        var result = mergerWithoutDeletions.MergeSchemaAndPackage(schemaWithIndex, scenario.Package);
        
        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var indexesAfter = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        indexesAfter.Count.ShouldBe(1, "Index should NOT be removed when AllowDeletions=false");
        indexesAfter[0].Id.ShouldBe(originalIndexId, "Same index should still exist with same ID");
        indexesAfter[0].Name.ShouldBe("IX_Customers_Email_Unique");
    }

    [Fact]
    public void MergeSchemaAndPackage_InclusiveImportAfterIndexRemoval_OnlyRemovesIndexFromImportedTable()
    {
        // Arrange - Start with two tables, each with an index
        var schemaWithTwoTables = DatabaseSchemas.WithCustomersAndOrders();
        schemaWithTwoTables.Tables[0].Indexes = 
        [
            Indexes.UniqueEmailIndex() // On Customers table
        ];
        schemaWithTwoTables.Tables[1].Indexes = 
        [
            Indexes.NonClusteredCompositeIndex() // On Orders table
        ];
        
        var scenario = ScenarioComposer.SchemaOnly(schemaWithTwoTables);
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesWithDeletions());
        
        // Initial import with both indexes
        merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Verify both indexes were created
        var indexesBefore = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        indexesBefore.Count.ShouldBe(2);
        var customersIndex = indexesBefore.Single(i => i.Name == "IX_Customers_Email_Unique");
        var ordersIndex = indexesBefore.Single(i => i.Name == "IX_Orders_CustomerId_OrderDate");
        
        // Now simulate inclusive import: only Orders table, and its index is removed
        var schemaOnlyOrders = DatabaseSchemas.WithCustomersAndOrders();
        schemaOnlyOrders.Tables = schemaOnlyOrders.Tables.Where(t => t.Name == "Orders").ToList();
        schemaOnlyOrders.Tables[0].Indexes = []; // Orders table has no indexes
        
        // Act - Re-import only Orders table after its index removal
        var result = merger.MergeSchemaAndPackage(schemaOnlyOrders, scenario.Package);
        
        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var indexesAfter = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        
        // Only Orders index should be removed, Customers index should remain
        indexesAfter.Count.ShouldBe(1, "Only the Orders index should be removed, Customers index should remain");
        indexesAfter[0].Id.ShouldBe(customersIndex.Id, "Customers index should be preserved (table not imported)");
        indexesAfter[0].Name.ShouldBe("IX_Customers_Email_Unique");
        
        // Verify Orders index is actually gone
        scenario.Package.Classes.ShouldNotContain(c => c.Id == ordersIndex.Id, "Orders index should be removed");
    }

    [Fact]
    public void MergeSchemaAndPackage_MultipleIndexesRemoved_RemovesAllObsoleteIndexes()
    {
        // Arrange - Start with table that has multiple indexes
        var schemaWithIndexes = DatabaseSchemas.WithSimpleUsersTable();
        schemaWithIndexes.Tables[0].Indexes =
        [
            Indexes.UniqueEmailIndex(),
            Indexes.NonClusteredCompositeIndex()
        ];
        
        var scenario = ScenarioComposer.SchemaOnly(schemaWithIndexes);
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesWithDeletions());
        
        // Initial import with indexes
        merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Verify indexes were created
        var indexesBefore = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        indexesBefore.Count.ShouldBe(2);
        var index1Id = indexesBefore[0].Id;
        var index2Id = indexesBefore[1].Id;
        
        // Now remove all indexes from the schema
        schemaWithIndexes.Tables[0].Indexes = [];
        
        // Act - Re-import without any indexes
        var result = merger.MergeSchemaAndPackage(schemaWithIndexes, scenario.Package);
        
        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var indexesAfter = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        indexesAfter.ShouldBeEmpty("All indexes should have been removed");
        
        // Verify both specific indexes are gone
        scenario.Package.Classes.ShouldNotContain(c => c.Id == index1Id);
        scenario.Package.Classes.ShouldNotContain(c => c.Id == index2Id);
    }

    [Fact]
    public void MergeSchemaAndPackage_IndexRemovedButOthersRemain_RemovesOnlyDeletedIndex()
    {
        // Arrange - Start with table that has two indexes
        var schemaWithIndexes = DatabaseSchemas.WithSimpleUsersTable();
        schemaWithIndexes.Tables[0].Indexes =
        [
            Indexes.UniqueEmailIndex(),
            Indexes.NonClusteredCompositeIndex()
        ];
        
        var scenario = ScenarioComposer.SchemaOnly(schemaWithIndexes);
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesWithDeletions());
        
        // Initial import with two indexes
        merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Verify indexes were created
        var indexesBefore = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        indexesBefore.Count.ShouldBe(2);
        var emailIndex = indexesBefore.Single(i => i.Name == "IX_Customers_Email_Unique");
        var compositeIndex = indexesBefore.Single(i => i.Name == "IX_Orders_CustomerId_OrderDate");
        
        // Now remove only one index from the schema
        schemaWithIndexes.Tables[0].Indexes =
        [
            Indexes.UniqueEmailIndex() // Keep this one
            // NonClusteredCompositeIndex is removed
        ];
        
        // Act - Re-import with only one index
        var result = merger.MergeSchemaAndPackage(schemaWithIndexes, scenario.Package);
        
        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var indexesAfter = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Index")
            .ToList();
        indexesAfter.Count.ShouldBe(1, "Only one index should remain");
        indexesAfter[0].Id.ShouldBe(emailIndex.Id, "The unique email index should remain");
        indexesAfter[0].Name.ShouldBe("IX_Customers_Email_Unique");
        
        // Verify the composite index is gone
        scenario.Package.Classes.ShouldNotContain(c => c.Id == compositeIndex.Id, "Composite index should be removed");
    }

    [Fact]
    public void MergeSchemaAndPackage_IndexStoredAsChildElementRemoved_RemovesIndexFromChildElements()
    {
        // Arrange - Start with table that has an index
        var schemaWithIndex = DatabaseSchemas.WithSimpleUsersTable();
        schemaWithIndex.Tables[0].Indexes =
        [
            Indexes.UniqueEmailIndex()
        ];
        
        var scenario = ScenarioComposer.SchemaOnly(schemaWithIndex);
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesWithDeletions());
        
        // Initial import with index
        merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Manually move the index from package.Classes to the parent class's ChildElements
        // (simulating indexes stored as nested elements, which is what the user reported)
        var indexInPackage = scenario.Package.Classes
            .Single(c => c.SpecializationType == "Index");
        var parentClass = scenario.Package.Classes
            .Single(c => c.Id == indexInPackage.ParentFolderId);
        
        scenario.Package.Classes.Remove(indexInPackage);
        parentClass.ChildElements.Add(indexInPackage);
        
        // Verify index is now in ChildElements
        parentClass.ChildElements.ShouldContain(e => e.SpecializationType == "Index");
        var indexId = indexInPackage.Id;
        
        // Now remove the index from the schema
        schemaWithIndex.Tables[0].Indexes = [];
        
        // Act - Re-import without the index
        var result = merger.MergeSchemaAndPackage(schemaWithIndex, scenario.Package);
        
        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Verify index was removed from ChildElements
        parentClass.ChildElements.ShouldNotContain(e => e.Id == indexId, 
            "Index should have been removed from ChildElements when it no longer exists in database");
        parentClass.ChildElements.ShouldNotContain(e => e.SpecializationType == "Index",
            "No index should remain in ChildElements");
    }

    private static IEnumerable<ElementPersistable> GetClasses(PackageModelPersistable package) =>
        package.Classes.Where(c =>
            string.Equals(c.SpecializationType, ClassModel.SpecializationType, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<string> GetAttributeNames(ElementPersistable element) =>
        element.ChildElements.Where(p => p.SpecializationTypeId == AttributeModel.SpecializationTypeId)
            .Select(a => a.Name);

    private static ElementPersistable GetClassById(IEnumerable<ElementPersistable> classes, string id) =>
        classes.Single(c => c.Id == id);

    #region Duplicate Foreign Key Tests

    [Fact]
    public void MergeSchemaAndPackage_DuplicateFKs_KeepsFirstEncountered()
    {
        // Arrange - Package has auto-generated FK, schema has both auto-generated and named FK
        var scenario = ScenarioComposer.Create(
            DatabaseSchemas.WithItemDuplicateFKs(),
            PackageModels.WithItemAutoGeneratedFK());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Get the existing association ID before merge
        var existingAssociation = scenario.Package.Associations.ShouldHaveSingleItem();
        var existingAssociationId = existingAssociation.Id;
        var existingTargetEndId = existingAssociation.TargetEnd.Id;
        var existingExternalRef = existingAssociation.ExternalReference;

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Should still have only one association (duplicate was skipped)
        scenario.Package.Associations.ShouldHaveSingleItem();
        
        // Association should remain unchanged (first encountered FK is kept)
        var association = scenario.Package.Associations.Single();
        association.Id.ShouldBe(existingAssociationId, "Association ID should not change");
        association.TargetEnd.Id.ShouldBe(existingTargetEndId, "Target end ID should not change");
        association.ExternalReference.ShouldBe(existingExternalRef, "External reference should remain unchanged - first FK is kept");
        
        // Should have a warning about the duplicate
        result.Warnings.ShouldContain(w => 
            w.Contains("Duplicate foreign key detected") && 
            w.Contains("first encountered") &&
            (w.Contains("FK__Item__Category__5F6C19D1") || w.Contains("FK_Item_Category")),
            "Should warn about duplicate FKs and keeping first encountered");
    }

    [Fact]
    public void MergeSchemaAndPackage_DuplicateAutoGeneratedFKs_KeepsFirstEncountered()
    {
        // Arrange - Fresh import with duplicate auto-generated FKs
        var scenario = ScenarioComposer.Create(
            DatabaseSchemas.WithItemDuplicateAutoGeneratedFKs(),
            PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Should have only one association (second duplicate was skipped)
        scenario.Package.Associations.ShouldHaveSingleItem();
        
        // Should have a warning about the duplicate
        result.Warnings.ShouldContain(w => 
            w.Contains("Duplicate foreign key detected") &&
            w.Contains("first encountered"),
            "Should warn about duplicate FKs and keeping first encountered");
    }

    [Fact]
    public void MergeSchemaAndPackage_DuplicateNamedFKs_KeepsFirstEncountered()
    {
        // Arrange - Fresh import with duplicate named FKs
        var scenario = ScenarioComposer.Create(
            DatabaseSchemas.WithItemDuplicateNamedFKs(),
            PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Should have only one association (second duplicate was skipped)
        scenario.Package.Associations.ShouldHaveSingleItem();
        
        // Should have a warning about the duplicate
        result.Warnings.ShouldContain(w => 
            w.Contains("Duplicate foreign key detected") &&
            w.Contains("first encountered"),
            "Should warn about duplicate FKs and keeping first encountered");
    }

    [Fact]
    public void MergeSchemaAndPackage_DuplicateFKs_IdempotentOnReimport()
    {
        // Arrange - Package has auto-generated FK, schema has both FKs
        var scenario = ScenarioComposer.Create(
            DatabaseSchemas.WithItemDuplicateFKs(),
            PackageModels.WithItemAutoGeneratedFK());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // First merge - should keep the existing auto-generated FK
        var result1 = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        result1.IsSuccessful.ShouldBeTrue();
        
        var associationAfterFirst = scenario.Package.Associations.Single();
        var associationIdAfterFirst = associationAfterFirst.Id;
        var externalRefAfterFirst = associationAfterFirst.ExternalReference;

        // Act - Second merge with same data
        var result2 = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result2.IsSuccessful.ShouldBeTrue();
        
        // Should still have only one association
        scenario.Package.Associations.ShouldHaveSingleItem();
        
        // Association should remain unchanged (idempotent)
        var associationAfterSecond = scenario.Package.Associations.Single();
        associationAfterSecond.Id.ShouldBe(associationIdAfterFirst, "Association ID should remain stable");
        associationAfterSecond.ExternalReference.ShouldBe(externalRefAfterFirst, "External reference should remain stable - first FK is always kept");
        
        // Should still warn about the duplicate
        result2.Warnings.ShouldContain(w => w.Contains("Duplicate foreign key detected"));
    }

    [Fact]
    public void MergeSchemaAndPackage_DuplicateFKs_OnlyOneAssociationCreated()
    {
        // Arrange
        var scenario = ScenarioComposer.Create(
            DatabaseSchemas.WithItemDuplicateFKs(),
            PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.TablesOnly());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Verify we have exactly 2 classes (Item and Category)
        var classes = GetClasses(scenario.Package).ToList();
        classes.Count.ShouldBe(2);
        classes.ShouldContain(c => c.Name == "Item");
        classes.ShouldContain(c => c.Name == "Category");
        
        // Verify we have exactly 1 association (not 2)
        scenario.Package.Associations.ShouldHaveSingleItem();
        
        // Verify the association points from Item to Category
        var association = scenario.Package.Associations.Single();
        association.TargetEnd.Name.ShouldBe("Category");
        
        // Should have warning about duplicate
        result.Warnings.ShouldContain(w => w.Contains("Duplicate foreign key detected"));
    }

    #endregion

    #region Stored Procedure Mapping Idempotency Tests

    [Fact]
    public void MergeSchemaAndPackage_StoredProcWithOutputParams_CreatesInvocationMappings()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.TestWithOutParam()]
        };
        var scenario = ScenarioComposer.Create(schema, PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.StoredProceduresAsOperations());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Should have created the stored procedure invocation association
        scenario.Package.Associations.ShouldHaveSingleItem();
        var association = scenario.Package.Associations.Single();
        association.AssociationType.ShouldBe("Stored Procedure Invocation");
        
        // Should have two mappings: Invocation and Result
        var mappings = association.TargetEnd.Mappings;
        mappings.ShouldNotBeNull();
        mappings.Count.ShouldBe(2);
        mappings.ShouldContain(m => m.Type == "Stored Procedure Invocation");
        mappings.ShouldContain(m => m.Type == "Stored Procedure Result");
        
        // Result mapping should have mapped ends for result set and output parameter
        var resultMapping = mappings.Single(m => m.Type == "Stored Procedure Result");
        resultMapping.MappedEnds.Count.ShouldBeGreaterThan(1);
        resultMapping.MappedEnds.ShouldContain(e => e.MappingExpression.Contains("result"));
        resultMapping.MappedEnds.ShouldContain(e => e.MappingExpression.Contains("errorMessage"));
    }

    [Fact]
    public void MergeSchemaAndPackage_StoredProcReimported_MappingIdsRemainStable()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.TestWithOutParam()]
        };
        var scenario = ScenarioComposer.Create(schema, PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.StoredProceduresAsOperations());
        
        // Initial import
        merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Capture all IDs
        var association = scenario.Package.Associations.Single();
        var associationId = association.Id;
        var targetEndId = association.TargetEnd.Id;
        var mappingIds = association.TargetEnd.Mappings.Select(m => (m.Type, m.TypeId)).ToList();
        var mappedEndExpressions = association.TargetEnd.Mappings
            .SelectMany(m => m.MappedEnds.Select(e => e.MappingExpression))
            .ToList();
        
        // Capture path target IDs from all mappings
        var allPathTargetIds = association.TargetEnd.Mappings
            .SelectMany(m => m.MappedEnds)
            .SelectMany(e => e.TargetPath.Select(p => (p.Name, p.Id, p.SpecializationId)))
            .ToList();
        
        // Capture TypeReference IDs from static-mappable path segments
        var staticMappableTypeRefIds = association.TargetEnd.Mappings
            .SelectMany(m => m.MappedEnds)
            .SelectMany(e => e.Sources ?? Enumerable.Empty<Intent.IArchitect.Agent.Persistence.Model.Mappings.ElementToElementMappedEndSourcePersistable>())
            .SelectMany(s => s.Path)
            .Where(p => p.Type == "static-mappable" && p.TypeReference != null)
            .Select(p => (p.Name, TypeRefId: p.TypeReference.Id))
            .ToList();

        // Act - Re-import with identical data
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Verify association ID is stable
        var associationAfter = scenario.Package.Associations.Single();
        associationAfter.Id.ShouldBe(associationId, "Association ID should remain unchanged");
        associationAfter.TargetEnd.Id.ShouldBe(targetEndId, "Target end ID should remain unchanged");
        
        // Verify mapping IDs are stable
        var mappingIdsAfter = associationAfter.TargetEnd.Mappings.Select(m => (m.Type, m.TypeId)).ToList();
        mappingIdsAfter.ShouldBe(mappingIds, "Mapping Type/TypeId pairs should remain unchanged");
        
        // Verify mapped end expressions are stable
        var mappedEndExpressionsAfter = associationAfter.TargetEnd.Mappings
            .SelectMany(m => m.MappedEnds.Select(e => e.MappingExpression))
            .ToList();
        mappedEndExpressionsAfter.ShouldBe(mappedEndExpressions, "Mapped end expressions should remain unchanged");
        
        // Verify path target IDs are stable
        var allPathTargetIdsAfter = associationAfter.TargetEnd.Mappings
            .SelectMany(m => m.MappedEnds)
            .SelectMany(e => e.TargetPath.Select(p => (p.Name, p.Id, p.SpecializationId)))
            .ToList();
        allPathTargetIdsAfter.ShouldBe(allPathTargetIds, "All path target IDs should remain unchanged");
        
        // Verify TypeReference IDs in static-mappable segments are stable (THE BUG WE FIXED)
        var staticMappableTypeRefIdsAfter = associationAfter.TargetEnd.Mappings
            .SelectMany(m => m.MappedEnds)
            .SelectMany(e => e.Sources ?? Enumerable.Empty<Intent.IArchitect.Agent.Persistence.Model.Mappings.ElementToElementMappedEndSourcePersistable>())
            .SelectMany(s => s.Path)
            .Where(p => p.Type == "static-mappable" && p.TypeReference != null)
            .Select(p => (p.Name, TypeRefId: p.TypeReference.Id))
            .ToList();
        staticMappableTypeRefIdsAfter.ShouldBe(staticMappableTypeRefIds, "TypeReference IDs in static-mappable paths should remain unchanged");
    }

    [Fact]
    public void MergeSchemaAndPackage_StoredProcOutputParamRenamed_UpdatesExpression()
    {
        // Arrange - Initial import with ErrorMessage parameter
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.TestWithOutParam()]
        };
        var scenario = ScenarioComposer.Create(schema, PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.StoredProceduresAsOperations());
        merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Capture association ID
        var association = scenario.Package.Associations.Single();
        var associationId = association.Id;
        var resultMapping = association.TargetEnd.Mappings.Single(m => m.Type == "Stored Procedure Result");
        var originalCount = resultMapping.MappedEnds.Count;
        
        // Now change parameter name to ErrorCode
        var modifiedProc = StoredProcedures.TestWithOutParam();
        modifiedProc.Parameters[1].Name = "@ErrorCode";
        scenario.Schema.StoredProcedures = [modifiedProc];

        // Act - Re-import with renamed parameter
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var associationAfter = scenario.Package.Associations.Single();
        associationAfter.Id.ShouldBe(associationId, "Association ID should remain stable");
        
        var resultMappingAfter = associationAfter.TargetEnd.Mappings.Single(m => m.Type == "Stored Procedure Result");
        
        // Verify the mapping count remains stable (not growing indefinitely)
        resultMappingAfter.MappedEnds.Count.ShouldBe(originalCount, 
            "Should have same number of mapped ends - the wrapper DC is regenerated on each import");
    }

    [Fact]
    public void MergeSchemaAndPackage_MultipleStoredProcsReimported_EachMappingRemainsStable()
    {
        // Arrange - Import multiple stored procedures
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [
                StoredProcedures.TestWithOutParam(),
                StoredProcedures.CreateCustomer(),
                StoredProcedures.UpdateCustomerBalance()
            ]
        };
        var scenario = ScenarioComposer.Create(schema, PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.StoredProceduresAsOperations());
        merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Capture IDs for each association
        var associationData = scenario.Package.Associations.Select(a => new
        {
            AssociationId = a.Id,
            TargetEndId = a.TargetEnd.Id,
            MappingCount = a.TargetEnd.Mappings.Count,
            MappedEndCount = a.TargetEnd.Mappings.Sum(m => m.MappedEnds.Count)
        }).ToList();

        // Act - Re-import all with identical data
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var associationDataAfter = scenario.Package.Associations.Select(a => new
        {
            AssociationId = a.Id,
            TargetEndId = a.TargetEnd.Id,
            MappingCount = a.TargetEnd.Mappings.Count,
            MappedEndCount = a.TargetEnd.Mappings.Sum(m => m.MappedEnds.Count)
        }).ToList();
        
        associationDataAfter.ShouldBe(associationData, "All association data should remain unchanged across re-import");
    }

    [Fact]
    public void MergeSchemaAndPackage_StoredProcWithoutOutputParams_DoesNotCreateAssociation()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.GetCustomerNoOutParams()] // No output params
        };
        var scenario = ScenarioComposer.Create(schema, PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.StoredProceduresAsOperations());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Should not create association when there are no output parameters
        scenario.Package.Associations.ShouldBeEmpty("No association should be created for stored procedures without output parameters");
    }

    [Fact]
    public void MergeSchemaAndPackage_StoredProcPathSegmentIds_RemainStableOnReimport()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.TestWithOutParam()]
        };
        var scenario = ScenarioComposer.Create(schema, PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.StoredProceduresAsOperations());
        merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);
        
        // Capture all path segment details from Result mapping
        var association = scenario.Package.Associations.Single();
        var resultMapping = association.TargetEnd.Mappings.Single(m => m.Type == "Stored Procedure Result");
        var errorMsgEnd = resultMapping.MappedEnds.Single(e => e.MappingExpression.Contains("errorMessage"));
        var sourcePath = errorMsgEnd.Sources.First().Path;
        
        var pathSegments = sourcePath.Select(p => new
        {
            p.Name,
            p.Id,
            p.SpecializationId,
            p.Type,
            p.Specialization,
            TypeRefId = p.TypeReference?.Id
        }).ToList();

        // Act - Re-import
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        var associationAfter = scenario.Package.Associations.Single();
        var resultMappingAfter = associationAfter.TargetEnd.Mappings.Single(m => m.Type == "Stored Procedure Result");
        var errorMsgEndAfter = resultMappingAfter.MappedEnds.Single(e => e.MappingExpression.Contains("errorMessage"));
        var sourcePathAfter = errorMsgEndAfter.Sources.First().Path;
        
        var pathSegmentsAfter = sourcePathAfter.Select(p => new
        {
            p.Name,
            p.Id,
            p.SpecializationId,
            p.Type,
            p.Specialization,
            TypeRefId = p.TypeReference?.Id
        }).ToList();
        
        // All path segments should have identical IDs, SpecializationIds, and TypeReference IDs
        pathSegmentsAfter.ShouldBe(pathSegments, "All path segment IDs and TypeReference IDs should remain stable");
    }

    [Fact]
    public async Task MergeSchemaAndPackage_StoredProcWithUserDefinedTableType_CreatesDataContractWithUdtSettingsStereotype()
    {
        // Arrange
        var schema = new DatabaseSchema
        {
            DatabaseName = "TestDatabase",
            Tables = [],
            Views = [],
            StoredProcedures = [StoredProcedures.WithUserDefinedTableTypeParameter()]
        };
        var scenario = ScenarioComposer.Create(schema, PackageModels.Empty());
        var merger = new DbSchemaIntentMetadataMerger(ImportConfigurations.StoredProceduresAsOperations());

        // Act
        var result = merger.MergeSchemaAndPackage(scenario.Schema, scenario.Package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Verify DataContract was created for the UDT
        var dataContracts = scenario.Package.Classes
            .Where(c => c.SpecializationType == "Data Contract")
            .ToList();
        dataContracts.Count.ShouldBeGreaterThan(0);
        
        var dataContract = dataContracts.FirstOrDefault(c => c.Name == "OrderTableTypeModel");
        dataContract.ShouldNotBeNull();
        
        // Verify the UDT Settings stereotype is applied
        var udtSettingsStereotype = dataContract.Stereotypes.FirstOrDefault(s => 
            s.DefinitionId == "43937e01-7079-4ab1-b5c4-b3cb0dee9dcd");
        udtSettingsStereotype.ShouldNotBeNull();
        
        // Verify the Name property is set to the original SQL name
        var nameProp = udtSettingsStereotype.Properties.FirstOrDefault(p => 
            p.DefinitionId == "fe47cbcb-5976-462c-8f85-7dbffcfea97d");
        nameProp.ShouldNotBeNull();
        nameProp.Value.ShouldBe("OrderTableType");
        
        // Verify the UDT columns are created as attributes
        var attributes = dataContract.ChildElements.Where(e => e.SpecializationType == "Attribute").ToList();
        attributes.Count.ShouldBe(4);
    }



    #endregion
}