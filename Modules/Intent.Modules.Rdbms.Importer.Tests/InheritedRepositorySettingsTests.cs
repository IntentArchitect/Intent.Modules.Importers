using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Shouldly;

namespace Intent.Modules.Rdbms.Importer.Tests;

/// <summary>
/// The repository-level remembered settings (e.g. a previously saved "Only for me" value) are the
/// first fallback tier for a blank Connection String/Database Type - tried before the package-level
/// Database Import settings in <see cref="SettingsHelper.ApplyInheritedDbSettings"/>.
/// </summary>
public class InheritedRepositorySettingsTests
{
    private static RepositoryImportResolvedSettings RememberedSettings() => new()
    {
        ConnectionString = "Data Source=remembered;Database=Remembered;",
        DatabaseType = nameof(DatabaseType.PostgreSQL),
        StoredProcedureType = "RepositoryOperation"
    };

    [Fact]
    public void ApplyInheritedRepositorySettings_ConnectionStringEntered_KeepsEnteredConnectionString()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;Database=Entered;"
        };

        SettingsHelper.ApplyInheritedRepositorySettings(importModel, RememberedSettings());

        importModel.ConnectionString.ShouldBe("Data Source=entered;Database=Entered;");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ApplyInheritedRepositorySettings_ConnectionStringBlank_UsesRememberedConnectionString(string? entered)
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = entered!
        };

        SettingsHelper.ApplyInheritedRepositorySettings(importModel, RememberedSettings());

        importModel.ConnectionString.ShouldBe("Data Source=remembered;Database=Remembered;");
    }

    [Fact]
    public void ApplyInheritedRepositorySettings_DatabaseTypeSelected_KeepsSelectedDatabaseType()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;",
            DatabaseType = DatabaseType.SqlServer
        };

        SettingsHelper.ApplyInheritedRepositorySettings(importModel, RememberedSettings());

        importModel.DatabaseType.ShouldBe(DatabaseType.SqlServer);
    }

    [Fact]
    public void ApplyInheritedRepositorySettings_DatabaseTypeNotSelected_UsesRememberedDatabaseType()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;",
            DatabaseType = null
        };

        SettingsHelper.ApplyInheritedRepositorySettings(importModel, RememberedSettings());

        importModel.DatabaseType.ShouldBe(DatabaseType.PostgreSQL);
    }

    [Fact]
    public void ApplyInheritedRepositorySettings_StoredProcedureTypeSelected_KeepsSelectedStoredProcedureType()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;",
            StoredProcedureType = "RepositoryOperationMapping"
        };

        SettingsHelper.ApplyInheritedRepositorySettings(importModel, RememberedSettings());

        importModel.StoredProcedureType.ShouldBe("RepositoryOperationMapping");
    }

    [Fact]
    public void ApplyInheritedRepositorySettings_StoredProcedureTypeBlank_UsesRememberedStoredProcedureType()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;",
            StoredProcedureType = null
        };

        SettingsHelper.ApplyInheritedRepositorySettings(importModel, RememberedSettings());

        importModel.StoredProcedureType.ShouldBe("RepositoryOperation");
    }

    [Fact]
    public void ApplyInheritedRepositorySettings_InheritedDatabaseTypeUnparseable_LeavesDatabaseTypeNull()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;"
        };

        SettingsHelper.ApplyInheritedRepositorySettings(importModel, new RepositoryImportResolvedSettings
        {
            ConnectionString = "Data Source=remembered;",
            DatabaseType = "NotADatabaseType",
            StoredProcedureType = "Default"
        });

        importModel.DatabaseType.ShouldBeNull();
    }
}
