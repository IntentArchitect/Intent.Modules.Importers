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

    private static IEnumerable<ElementPersistable> GetClasses(PackageModelPersistable package) =>
        package.Classes.Where(c =>
            string.Equals(c.SpecializationType, ClassModel.SpecializationType, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<string> GetAttributeNames(ElementPersistable element) =>
        element.ChildElements.Where(p => p.SpecializationTypeId == AttributeModel.SpecializationTypeId)
            .Select(a => a.Name);

    private static ElementPersistable GetClassById(IEnumerable<ElementPersistable> classes, string id) =>
        classes.Single(c => c.Id == id);

}