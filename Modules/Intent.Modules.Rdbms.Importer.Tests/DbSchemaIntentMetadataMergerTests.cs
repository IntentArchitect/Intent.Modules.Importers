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
}