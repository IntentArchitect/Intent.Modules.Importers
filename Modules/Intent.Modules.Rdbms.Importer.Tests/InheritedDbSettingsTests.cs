using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Shouldly;

namespace Intent.Modules.Rdbms.Importer.Tests;

/// <summary>
/// "Inherit Database Settings" fills in what the stored procedure import dialog left blank - it does
/// not override what the user actually typed. Before this was fixed, entering a connection string
/// while inheriting was silently discarded in favour of the package-level one.
/// </summary>
public class InheritedDbSettingsTests
{
    private static DatabaseImportResolvedSettings InheritedSettings() => new()
    {
        ConnectionString = "Data Source=inherited;Database=Inherited;",
        DatabaseType = nameof(DatabaseType.PostgreSQL),
        StoredProcedureType = "RepositoryOperation"
    };

    [Fact]
    public void ApplyInheritedDbSettings_ConnectionStringEntered_KeepsEnteredConnectionString()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;Database=Entered;"
        };

        SettingsHelper.ApplyInheritedDbSettings(importModel, InheritedSettings());

        importModel.ConnectionString.ShouldBe("Data Source=entered;Database=Entered;");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ApplyInheritedDbSettings_ConnectionStringBlank_UsesInheritedConnectionString(string? entered)
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = entered!
        };

        SettingsHelper.ApplyInheritedDbSettings(importModel, InheritedSettings());

        importModel.ConnectionString.ShouldBe("Data Source=inherited;Database=Inherited;");
    }

    [Fact]
    public void ApplyInheritedDbSettings_DatabaseTypeSelected_KeepsSelectedDatabaseType()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;",
            DatabaseType = DatabaseType.SqlServer
        };

        SettingsHelper.ApplyInheritedDbSettings(importModel, InheritedSettings());

        importModel.DatabaseType.ShouldBe(DatabaseType.SqlServer);
    }

    [Fact]
    public void ApplyInheritedDbSettings_DatabaseTypeNotSelected_UsesInheritedDatabaseType()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;",
            DatabaseType = null
        };

        SettingsHelper.ApplyInheritedDbSettings(importModel, InheritedSettings());

        importModel.DatabaseType.ShouldBe(DatabaseType.PostgreSQL);
    }

    [Fact]
    public void ApplyInheritedDbSettings_StoredProcedureTypeSelected_KeepsSelectedStoredProcedureType()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;",
            StoredProcedureType = "RepositoryOperationMapping"
        };

        SettingsHelper.ApplyInheritedDbSettings(importModel, InheritedSettings());

        importModel.StoredProcedureType.ShouldBe("RepositoryOperationMapping");
    }

    [Fact]
    public void ApplyInheritedDbSettings_StoredProcedureTypeBlank_UsesInheritedStoredProcedureType()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;",
            StoredProcedureType = null
        };

        SettingsHelper.ApplyInheritedDbSettings(importModel, InheritedSettings());

        importModel.StoredProcedureType.ShouldBe("RepositoryOperation");
    }

    [Fact]
    public void ApplyInheritedDbSettings_EverythingEntered_LeavesAllEnteredValuesIntact()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;Database=Entered;",
            DatabaseType = DatabaseType.SqlServer,
            StoredProcedureType = "StoredProcedureElement"
        };

        SettingsHelper.ApplyInheritedDbSettings(importModel, InheritedSettings());

        importModel.ConnectionString.ShouldBe("Data Source=entered;Database=Entered;");
        importModel.DatabaseType.ShouldBe(DatabaseType.SqlServer);
        importModel.StoredProcedureType.ShouldBe("StoredProcedureElement");
    }

    [Fact]
    public void ApplyInheritedDbSettings_InheritedDatabaseTypeUnparseable_LeavesDatabaseTypeNull()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;"
        };

        SettingsHelper.ApplyInheritedDbSettings(importModel, new DatabaseImportResolvedSettings
        {
            ConnectionString = "Data Source=inherited;",
            DatabaseType = "NotADatabaseType",
            StoredProcedureType = "Default"
        });

        importModel.DatabaseType.ShouldBeNull();
    }
}
