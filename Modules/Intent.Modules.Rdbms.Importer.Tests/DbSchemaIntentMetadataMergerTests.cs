using Intent.Modelers.Domain.Api;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.Modules.Rdbms.Importer.Tests.TestData;
using Shouldly;

namespace Intent.Modules.Rdbms.Importer.Tests;

public class DbSchemaIntentMetadataMergerTests
{
    [Fact]
    public async Task MergeSchemaAndPackage_TableWithColumns_CreatesClassElementWithAttributes()
    {
        // Arrange
        var schema = DatabaseSchemas.WithSimpleUsersTable();
        var package = PackageModels.Empty();
        var config = ImportConfigurations.TablesOnly();
        var merger = new DbSchemaIntentMetadataMerger(config);

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert - basic smoke tests
        result.IsSuccessful.ShouldBeTrue();
        
        // Snapshot test the resulting class
        await Verify(package.Classes);
    }

    [Fact]
    public async Task MergeSchemaAndPackage_CustomerAndOrderTablesToEmptyPackage_CreatesClassesAndAssociation()
    {
        // Arrange
        var schema = DatabaseSchemas.WithCustomersAndOrders();
        var package = PackageModels.Empty();
        var config = ImportConfigurations.TablesOnly();
        var merger = new DbSchemaIntentMetadataMerger(config);

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Snapshot test - verify both classes and associations
        await Verify(new
        {
            Classes = package.Classes,
            Associations = package.Associations
        });
    }

    [Fact]
    public async Task MergeSchemaAndPackage_SameDataReimported_RemainsIdempotent()
    {
        // Arrange
        var schema = DatabaseSchemas.WithCustomersAndOrders();
        var package = PackageModels.WithCustomerAndOrderTables();
        var config = ImportConfigurations.TablesOnly();
        var merger = new DbSchemaIntentMetadataMerger(config);

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Verify no duplicates - should still have exactly 2 classes and 1 association
        await Verify(new
        {
            Classes = package.Classes,
            Associations = package.Associations
        });
    }

    [Fact]
    public async Task MergeSchemaAndPackage_NewTableDetected_AddsOnlyNewTable()
    {
        // Arrange - Package has Customer, schema now includes Customer + Product
        var schema = DatabaseSchemas.WithCustomerAndNewProduct();
        var package = PackageModels.WithCustomerTable();
        var config = ImportConfigurations.TablesOnly();
        var merger = new DbSchemaIntentMetadataMerger(config);

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Should now have 2 classes: existing Customer + new Product
        await Verify(package.Classes);
    }

    [Fact]
    public async Task MergeSchemaAndPackage_ExistingTableWithNewColumn_AddsNewAttribute()
    {
        // Arrange - Customer table exists with Id and Email, now has Address column added
        var schema = DatabaseSchemas.WithCustomerWithNewAddressColumn();
        var package = PackageModels.WithCustomerTable();
        var config = ImportConfigurations.TablesOnly();
        var merger = new DbSchemaIntentMetadataMerger(config);

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Customer should now have 3 attributes: Id, Email (existing), Address (new)
        await Verify(package.Classes);
    }

    [Fact]
    public async Task MergeSchemaAndPackage_ColumnRemovedWithAllowDeletionsFalse_KeepsAttribute()
    {
        // Arrange - Customer in package has Id and Email, but DB only has Id now
        var schema = DatabaseSchemas.WithCustomerMissingEmailColumn();
        var package = PackageModels.WithCustomerTable();
        var config = ImportConfigurations.TablesOnly(); // allowDeletions = false
        var merger = new DbSchemaIntentMetadataMerger(config);

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Email attribute should still be present (not deleted)
        await Verify(package.Classes);
    }

    [Fact]
    public async Task MergeSchemaAndPackage_ColumnRemovedWithAllowDeletionsTrue_RemovesAttribute()
    {
        // Arrange - Customer in package has Id and Email, but DB only has Id now
        var schema = DatabaseSchemas.WithCustomerMissingEmailColumn();
        var package = PackageModels.WithCustomerTable();
        var config = ImportConfigurations.TablesWithDeletions(); // allowDeletions = true
        var merger = new DbSchemaIntentMetadataMerger(config);

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Email attribute should be removed, only Id should remain
        await Verify(package.Classes);
    }

    [Fact]
    public async Task MergeSchemaAndPackage_NewTableWithForeignKeyToExistingTable_CreatesClassAndAssociation()
    {
        // Arrange - Customer exists in package, now importing Orders with FK to Customer
        var schema = DatabaseSchemas.WithOrderAndExistingCustomerReference();
        var package = PackageModels.WithExistingCustomer();
        var config = ImportConfigurations.TablesOnly();
        var merger = new DbSchemaIntentMetadataMerger(config);

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Should have 2 classes (Customer existing + Order new) and 1 association
        await Verify(new
        {
            Classes = package.Classes,
            Associations = package.Associations
        });
    }

    [Fact]
    public async Task MergeSchemaAndPackage_MultipleSchemas_ImportsTablesFromBothSchemas()
    {
        // Arrange - Tables from both [dbo] and [schema2]
        var schema = DatabaseSchemas.WithMultipleSchemas();
        var package = PackageModels.Empty();
        var config = ImportConfigurations.TablesOnly();
        var merger = new DbSchemaIntentMetadataMerger(config);

        // Act
        var result = merger.MergeSchemaAndPackage(schema, package);

        // Assert
        result.IsSuccessful.ShouldBeTrue();
        
        // Verify both tables imported with correct schema references
        await Verify(package.Classes);
    }
}