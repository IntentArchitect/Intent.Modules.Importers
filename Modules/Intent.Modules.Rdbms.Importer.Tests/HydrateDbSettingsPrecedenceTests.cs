using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Shouldly;

namespace Intent.Modules.Rdbms.Importer.Tests;

/// <summary>
/// SettingsHelper.HydrateDbSettings falls back through two tiers for whatever was left blank on the
/// stored procedure import dialog: the repository-level remembered settings first, then the
/// package-level Database Import settings - mirroring the dialog's own resolution in
/// strategy-stored-procedures-import.ts. Before this was fixed, a team that never ran the
/// package-level Database Import and only ever relied on repository-level remembered settings had
/// both fields wrongly forced to be required, and a blank submission failed to resolve at import time.
/// </summary>
public class HydrateDbSettingsPrecedenceTests : IDisposable
{
    private readonly string _packageFilePath;

    public HydrateDbSettingsPrecedenceTests()
    {
        _packageFilePath = CreateTempPackageFile();
    }

    public void Dispose()
    {
        if (File.Exists(_packageFilePath))
        {
            File.Delete(_packageFilePath);
        }
    }

    [Fact]
    public void HydrateDbSettings_OnlyRepositoryLevelSettingsPresent_UsesRepositoryLevelSettings()
    {
        WriteMetadata(new()
        {
            ["rdbms-import-repository:connectionString"] = "Data Source=repository;",
            ["rdbms-import-repository:databaseType"] = nameof(DatabaseType.PostgreSQL)
        });

        var importModel = new RepositoryImportModel
        {
            PackageFileName = _packageFilePath,
            ConnectionString = null!,
            DatabaseType = null
        };

        SettingsHelper.HydrateDbSettings(importModel);

        importModel.ConnectionString.ShouldBe("Data Source=repository;");
        importModel.DatabaseType.ShouldBe(DatabaseType.PostgreSQL);
    }

    [Fact]
    public void HydrateDbSettings_OnlyPackageLevelSettingsPresent_UsesPackageLevelSettings()
    {
        WriteMetadata(new()
        {
            ["rdbms-import:connectionString"] = "Data Source=package;",
            ["rdbms-import:databaseType"] = nameof(DatabaseType.PostgreSQL)
        });

        var importModel = new RepositoryImportModel
        {
            PackageFileName = _packageFilePath,
            ConnectionString = null!,
            DatabaseType = null
        };

        SettingsHelper.HydrateDbSettings(importModel);

        importModel.ConnectionString.ShouldBe("Data Source=package;");
        importModel.DatabaseType.ShouldBe(DatabaseType.PostgreSQL);
    }

    [Fact]
    public void HydrateDbSettings_BothLevelsPresent_RepositoryLevelWinsOverPackageLevel()
    {
        WriteMetadata(new()
        {
            ["rdbms-import-repository:connectionString"] = "Data Source=repository;",
            ["rdbms-import-repository:databaseType"] = nameof(DatabaseType.PostgreSQL),
            ["rdbms-import:connectionString"] = "Data Source=package;",
            ["rdbms-import:databaseType"] = nameof(DatabaseType.SqlServer)
        });

        var importModel = new RepositoryImportModel
        {
            PackageFileName = _packageFilePath,
            ConnectionString = null!,
            DatabaseType = null
        };

        SettingsHelper.HydrateDbSettings(importModel);

        importModel.ConnectionString.ShouldBe("Data Source=repository;");
        importModel.DatabaseType.ShouldBe(DatabaseType.PostgreSQL);
    }

    [Fact]
    public void HydrateDbSettings_EnteredValue_WinsOverBothFallbackTiers()
    {
        WriteMetadata(new()
        {
            ["rdbms-import-repository:connectionString"] = "Data Source=repository;",
            ["rdbms-import-repository:databaseType"] = nameof(DatabaseType.PostgreSQL),
            ["rdbms-import:connectionString"] = "Data Source=package;",
            ["rdbms-import:databaseType"] = nameof(DatabaseType.SqlServer)
        });

        var importModel = new RepositoryImportModel
        {
            PackageFileName = _packageFilePath,
            ConnectionString = "Data Source=entered;",
            DatabaseType = DatabaseType.SqlServer
        };

        SettingsHelper.HydrateDbSettings(importModel);

        importModel.ConnectionString.ShouldBe("Data Source=entered;");
        importModel.DatabaseType.ShouldBe(DatabaseType.SqlServer);
    }

    [Fact]
    public void HydrateDbSettings_NeitherLevelPresent_LeavesConnectionStringBlankAndUsesBuiltInDefaultDatabaseType()
    {
        var importModel = new RepositoryImportModel
        {
            PackageFileName = _packageFilePath,
            ConnectionString = null!,
            DatabaseType = null
        };

        SettingsHelper.HydrateDbSettings(importModel);

        // No connection string default exists at either fallback tier; the database type falls
        // all the way through to ResolveDatabaseImportSettings' own built-in default (SqlServer).
        importModel.ConnectionString.ShouldBeNullOrEmpty();
        importModel.DatabaseType.ShouldBe(DatabaseType.SqlServer);
    }

    private void WriteMetadata(Dictionary<string, string> metadata)
    {
        var entries = string.Join(string.Empty, metadata.Select(kvp => $"""<entry key="{kvp.Key}" value="{kvp.Value}" />"""));
        var xml = File.ReadAllText(_packageFilePath).Replace("<metadata />", $"<metadata>{entries}</metadata>");
        File.WriteAllText(_packageFilePath, xml);
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
