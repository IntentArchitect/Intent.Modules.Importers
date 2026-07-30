using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Shouldly;

namespace Intent.Modules.Rdbms.Importer.Tests;

/// <summary>
/// Under "Inherit Database Settings" the stored procedure import remembers connection settings the
/// user typed over the top of what it inherits, but must not write inherited values back out as a
/// per-repository override. These cover the two decisions that make that safe: telling an entered
/// value apart from an inherited one, and reading the persistence choice back off a user-local file.
/// </summary>
public class RepositoryInheritedOverrideTests
{
    [Fact]
    public void HasConnectionOverride_ConnectionStringEntered_IsTrue()
    {
        var entered = RepositoryEnteredSettings.From(new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;"
        });

        entered.HasConnectionOverride.ShouldBeTrue();
    }

    [Fact]
    public void HasConnectionOverride_DatabaseTypeSelected_IsTrue()
    {
        var entered = RepositoryEnteredSettings.From(new RepositoryImportModel
        {
            ConnectionString = null!,
            DatabaseType = DatabaseType.SqlServer
        });

        entered.HasConnectionOverride.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void HasConnectionOverride_NothingEntered_IsFalse(string? connectionString)
    {
        var entered = RepositoryEnteredSettings.From(new RepositoryImportModel
        {
            ConnectionString = connectionString!,
            DatabaseType = null
        });

        entered.HasConnectionOverride.ShouldBeFalse();
    }

    [Fact]
    public void HasConnectionOverride_OnlyStoredProcedureTypeEntered_IsFalse()
    {
        // Stored procedure type is not inherited from the Database Import settings, so on its own it
        // is not a connection override and must not pull the connection settings into user storage.
        var entered = RepositoryEnteredSettings.From(new RepositoryImportModel
        {
            ConnectionString = null!,
            DatabaseType = null,
            StoredProcedureType = "RepositoryOperationMapping"
        });

        entered.HasConnectionOverride.ShouldBeFalse();
    }

    [Fact]
    public void From_CapturesEnteredValuesBeforeHydration()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = "Data Source=entered;",
            DatabaseType = DatabaseType.SqlServer,
            StoredProcedureType = "StoredProcedureElement"
        };

        var entered = RepositoryEnteredSettings.From(importModel);

        // Hydration mutates the model afterwards; the snapshot must not follow it.
        SettingsHelper.ApplyInheritedDbSettings(importModel, new DatabaseImportResolvedSettings
        {
            ConnectionString = "Data Source=inherited;",
            DatabaseType = nameof(DatabaseType.PostgreSQL),
            StoredProcedureType = "RepositoryOperation"
        });

        entered.ConnectionString.ShouldBe("Data Source=entered;");
        entered.DatabaseType.ShouldBe(DatabaseType.SqlServer);
        entered.StoredProcedureType.ShouldBe("StoredProcedureElement");
    }

    [Fact]
    public void From_BlankModelHydrated_SnapshotStaysBlank()
    {
        var importModel = new RepositoryImportModel
        {
            ConnectionString = null!,
            DatabaseType = null
        };

        var entered = RepositoryEnteredSettings.From(importModel);

        SettingsHelper.ApplyInheritedDbSettings(importModel, new DatabaseImportResolvedSettings
        {
            ConnectionString = "Data Source=inherited;",
            DatabaseType = nameof(DatabaseType.PostgreSQL),
            StoredProcedureType = "RepositoryOperation"
        });

        // The model now holds inherited values, but nothing was entered - so nothing gets persisted
        // as a repository-level override and inheritance keeps working next time.
        importModel.ConnectionString.ShouldBe("Data Source=inherited;");
        entered.HasConnectionOverride.ShouldBeFalse();
    }

    [Fact]
    public void ResolveLocalRepositorySettingPersistence_InheritDbOverride_ResolvesToInheritDb()
    {
        var localSettings = new RepositoryImportLocalSettings
        {
            ConnectionString = "Data Source=entered;",
            SettingPersistence = nameof(RepositorySettingPersistence.InheritDb)
        };

        SettingsHelper.ResolveLocalRepositorySettingPersistence(localSettings)
            .ShouldBe(RepositorySettingPersistence.InheritDb);
    }

    [Fact]
    public void ResolveLocalRepositorySettingPersistence_UserLocal_ResolvesToUserLocal()
    {
        var localSettings = new RepositoryImportLocalSettings
        {
            SettingPersistence = nameof(RepositorySettingPersistence.UserLocal)
        };

        SettingsHelper.ResolveLocalRepositorySettingPersistence(localSettings)
            .ShouldBe(RepositorySettingPersistence.UserLocal);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("NotAPersistenceValue")]
    public void ResolveLocalRepositorySettingPersistence_MissingOrUnrecognised_FallsBackToUserLocal(string? stored)
    {
        var localSettings = new RepositoryImportLocalSettings
        {
            SettingPersistence = stored
        };

        SettingsHelper.ResolveLocalRepositorySettingPersistence(localSettings)
            .ShouldBe(RepositorySettingPersistence.UserLocal);
    }

    [Fact]
    public void ResolveLocalRepositorySettingPersistence_LegacyAllValue_NormalisesToSharedMetadata()
    {
        var localSettings = new RepositoryImportLocalSettings
        {
            SettingPersistence = nameof(RepositorySettingPersistence.All)
        };

        SettingsHelper.ResolveLocalRepositorySettingPersistence(localSettings)
            .ShouldBe(RepositorySettingPersistence.SharedMetadata);
    }
}
