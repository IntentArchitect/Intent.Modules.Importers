using System;
using System.IO;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Shouldly;

namespace Intent.Modules.Rdbms.Importer.Tests;

/// <summary>
/// Once a blank Connection String/Database Type always falls back to the package-level Database
/// Import settings - regardless of which Remember Settings option is selected - persistence must
/// still only capture what the user actually typed, never the value resolved via that fallback.
/// Before this was fixed, "Only for me" and the "Team-shared metadata" variants persisted whatever
/// SettingsHelper.HydrateDbSettings had already filled in, silently freezing the inherited value in
/// place as a permanent per-repository override.
/// </summary>
public class RepositoryPersistedSettingsTests : IDisposable
{
  private readonly string _packageFilePath;

  public RepositoryPersistedSettingsTests()
  {
    _packageFilePath = CreateTempPackageFile();
  }

  public void Dispose()
  {
    if (File.Exists(_packageFilePath))
    {
      File.Delete(_packageFilePath);
    }

    var userLocalPath = SettingsHelper.GetRepositoryUserLocalSettingsPath(_packageFilePath);
    if (File.Exists(userLocalPath))
    {
      File.Delete(userLocalPath);
    }
  }

  [Fact]
  public void PersistSettings_UserLocal_NothingEntered_PersistsBlankNotHydratedValues()
  {
    // Nothing was typed; SettingsHelper.HydrateDbSettings has already filled the model in from
    // the package-level Database Import settings by the time PersistSettings runs.
    var entered = new RepositoryEnteredSettings(null, null, null);
    var importModel = new RepositoryImportModel
    {
      PackageFileName = _packageFilePath,
      SettingPersistence = RepositorySettingPersistence.UserLocal,
      ConnectionString = "Data Source=inherited;",
      DatabaseType = DatabaseType.SqlServer,
      StoredProcedureType = "RepositoryOperation"
    };

    SettingsHelper.PersistSettings(importModel, entered);

    var userLocalPath = SettingsHelper.GetRepositoryUserLocalSettingsPath(_packageFilePath);
    File.Exists(userLocalPath).ShouldBeTrue();
    var persisted = System.Text.Json.JsonSerializer.Deserialize<RepositoryImportLocalSettings>(
      File.ReadAllText(userLocalPath), SerializationHelper.SerializerOptions);

    persisted.ShouldNotBeNull();
    persisted.ConnectionString.ShouldBeNullOrEmpty();
    persisted.DatabaseType.ShouldBeNullOrEmpty();
  }

  [Fact]
  public void PersistSettings_UserLocal_ValueEntered_PersistsEnteredValue()
  {
    var entered = new RepositoryEnteredSettings("Data Source=entered;", DatabaseType.PostgreSQL, "RepositoryOperationMapping");
    var importModel = new RepositoryImportModel
    {
      PackageFileName = _packageFilePath,
      SettingPersistence = RepositorySettingPersistence.UserLocal,
      ConnectionString = "Data Source=entered;",
      DatabaseType = DatabaseType.PostgreSQL,
      StoredProcedureType = "RepositoryOperationMapping"
    };

    SettingsHelper.PersistSettings(importModel, entered);

    var userLocalPath = SettingsHelper.GetRepositoryUserLocalSettingsPath(_packageFilePath);
    var persisted = System.Text.Json.JsonSerializer.Deserialize<RepositoryImportLocalSettings>(
      File.ReadAllText(userLocalPath), SerializationHelper.SerializerOptions);

    persisted.ShouldNotBeNull();
    persisted.ConnectionString.ShouldBe("Data Source=entered;");
    persisted.DatabaseType.ShouldBe(nameof(DatabaseType.PostgreSQL));
  }

  [Theory]
  [InlineData(RepositorySettingPersistence.SharedMetadata)]
  [InlineData(RepositorySettingPersistence.SharedMetadataSanitisedConnectionString)]
  [InlineData(RepositorySettingPersistence.SharedMetadataWithoutConnectionString)]
  public void PersistSettings_SharedMetadataVariant_NothingEntered_PersistsBlankNotHydratedConnectionString(
    RepositorySettingPersistence persistence)
  {
    // Hydration has already resolved a connection string/database type from the package-level
    // Database Import settings, but nothing was actually typed on the dialog.
    var entered = new RepositoryEnteredSettings(null, null, null);
    var importModel = new RepositoryImportModel
    {
      PackageFileName = _packageFilePath,
      SettingPersistence = persistence,
      ConnectionString = "Data Source=inherited;Password=secret;",
      DatabaseType = DatabaseType.SqlServer,
      StoredProcedureType = "RepositoryOperation"
    };

    SettingsHelper.PersistSettings(importModel, entered);

    var reloaded = PackageModelPersistable.Load(_packageFilePath);
    reloaded.GetMetadataValue("rdbms-import-repository:connectionString")
      .ShouldBeNullOrEmpty();
  }

  [Fact]
  public void PersistSettings_SharedMetadata_ValueEntered_PersistsEnteredConnectionString()
  {
    var entered = new RepositoryEnteredSettings("Data Source=entered;", DatabaseType.SqlServer, "RepositoryOperation");
    var importModel = new RepositoryImportModel
    {
      PackageFileName = _packageFilePath,
      SettingPersistence = RepositorySettingPersistence.SharedMetadata,
      ConnectionString = "Data Source=entered;",
      DatabaseType = DatabaseType.SqlServer,
      StoredProcedureType = "RepositoryOperation"
    };

    SettingsHelper.PersistSettings(importModel, entered);

    var reloaded = PackageModelPersistable.Load(_packageFilePath);
    reloaded.GetMetadataValue("rdbms-import-repository:connectionString")
      .ShouldBe("Data Source=entered;");
    reloaded.GetMetadataValue("rdbms-import-repository:databaseType")
      .ShouldBe(nameof(DatabaseType.SqlServer));
  }

  [Fact]
  public void PersistSettings_SharedMetadataSanitisedConnectionString_EnteredPassword_IsStripped()
  {
    var entered = new RepositoryEnteredSettings("Data Source=entered;Password=secret;", DatabaseType.SqlServer, null);
    var importModel = new RepositoryImportModel
    {
      PackageFileName = _packageFilePath,
      SettingPersistence = RepositorySettingPersistence.SharedMetadataSanitisedConnectionString,
      ConnectionString = "Data Source=entered;Password=secret;",
      DatabaseType = DatabaseType.SqlServer,
      StoredProcedureType = "Default"
    };

    SettingsHelper.PersistSettings(importModel, entered);

    var reloaded = PackageModelPersistable.Load(_packageFilePath);
    reloaded.GetMetadataValue("rdbms-import-repository:connectionString")
      .ShouldNotContain("secret");
  }

  [Fact]
  public void PersistSettings_SharedMetadataWithoutConnectionString_EnteredValue_ConnectionStringNotPersisted()
  {
    var entered = new RepositoryEnteredSettings("Data Source=entered;", DatabaseType.SqlServer, null);
    var importModel = new RepositoryImportModel
    {
      PackageFileName = _packageFilePath,
      SettingPersistence = RepositorySettingPersistence.SharedMetadataWithoutConnectionString,
      ConnectionString = "Data Source=entered;",
      DatabaseType = DatabaseType.SqlServer,
      StoredProcedureType = "Default"
    };

    SettingsHelper.PersistSettings(importModel, entered);

    var reloaded = PackageModelPersistable.Load(_packageFilePath);
    reloaded.GetMetadataValue("rdbms-import-repository:connectionString")
      .ShouldBeNullOrEmpty();
  }

  [Fact]
  public void ValidateHydratedDbSettings_ConnectionStringBlank_Throws()
  {
    var importModel = new RepositoryImportModel
    {
      ConnectionString = null!,
      DatabaseType = DatabaseType.SqlServer
    };

    Should.Throw<Exception>(() => SettingsHelper.ValidateHydratedDbSettings(importModel));
  }

  [Fact]
  public void ValidateHydratedDbSettings_DatabaseTypeNull_Throws()
  {
    var importModel = new RepositoryImportModel
    {
      ConnectionString = "Data Source=entered;",
      DatabaseType = null
    };

    Should.Throw<Exception>(() => SettingsHelper.ValidateHydratedDbSettings(importModel));
  }

  [Fact]
  public void ValidateHydratedDbSettings_BothPresent_DoesNotThrow()
  {
    var importModel = new RepositoryImportModel
    {
      ConnectionString = "Data Source=entered;",
      DatabaseType = DatabaseType.SqlServer
    };

    Should.NotThrow(() => SettingsHelper.ValidateHydratedDbSettings(importModel));
  }

  private static string CreateTempPackageFile()
  {
    var path = Path.Combine(Path.GetTempPath(), $"rdbms-import-test-{Guid.NewGuid()}.pkg.config");
    var packageId = Guid.NewGuid();
    var applicationId = Guid.NewGuid();
    File.WriteAllText(path, $"""
      <?xml version="1.0" encoding="utf-8"?>
      <package id="{packageId}" type="Domain Package" typeId="1a824508-4623-45d9-accc-f572091ade5a">
      <applicationId>{applicationId}</applicationId>
      <designerId>6ab29b31-27af-4f56-a67c-986d82097d63</designerId>
      <name>TestPackage</name>
      <isExternal>false</isExternal>
      <references />
      <stereotypes />
      <metadata />
      </package>
      """);

    return path;
  }
}
