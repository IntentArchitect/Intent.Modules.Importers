using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Serialization;
using Intent.IArchitect.CrossPlatform.IO;

using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.Utils;

namespace Intent.Modules.Rdbms.Importer.Tasks.Helpers;

internal static class SettingsHelper
{
    private static readonly UserLocalSettingsStore<DatabaseImportLocalSettings> DatabaseUserLocalStore = new(fileNameSuffix: null);
    private static readonly UserLocalSettingsStore<RepositoryImportLocalSettings> RepositoryUserLocalStore = new(fileNameSuffix: "storedprocs");

    public static void PersistSettings(DatabaseImportModel importModel)
    {
        ArgumentNullException.ThrowIfNull(importModel);

        Logging.Log.Info($"PackageFileName: {importModel.PackageFileName}");
        var package = LoadPackage(importModel.PackageFileName!);


        switch (NormalizeDatabaseSettingPersistence(importModel.SettingPersistence))
        {
            case DatabaseSettingPersistence.None:
                RemoveDatabaseMetadata(package);
                DatabaseUserLocalStore.Delete(importModel.PackageFileName!);
                break;
            case DatabaseSettingPersistence.UserLocal:
                DatabaseUserLocalStore.Save(importModel.PackageFileName!, CreateUserLocalSettings(importModel));
                RemoveDatabaseMetadata(package);
                break;

            case DatabaseSettingPersistence.SharedMetadata:
            case DatabaseSettingPersistence.SharedMetadataSanitisedConnectionString:
            case DatabaseSettingPersistence.SharedMetadataWithoutConnectionString:
                DatabaseUserLocalStore.Delete(importModel.PackageFileName!);
                PersistSharedDatabaseMetadata(package, importModel);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(importModel.SettingPersistence), importModel.SettingPersistence, "Unsupported database setting persistence.");
        }

        package.Save();
    }

    public static void PersistSettings(RepositoryImportModel importModel, RepositoryEnteredSettings entered)
    {
        ArgumentNullException.ThrowIfNull(importModel);
        ArgumentNullException.ThrowIfNull(entered);

        Logging.Log.Info($"PackageFileName: {importModel.PackageFileName}");
        var package = LoadPackage(importModel.PackageFileName!);

        var normalizedPersistence = NormalizeRepositorySettingPersistence(importModel.SettingPersistence);

        switch (normalizedPersistence)
        {
            case RepositorySettingPersistence.None:
                RemoveRepositoryMetadata(package);
                RepositoryUserLocalStore.Delete(importModel.PackageFileName!);
                break;
            case RepositorySettingPersistence.UserLocal:
                RepositoryUserLocalStore.Save(importModel.PackageFileName!, CreateRepositoryUserLocalSettings(importModel, normalizedPersistence));
                RemoveRepositoryMetadata(package);
                break;
            case RepositorySettingPersistence.InheritDb:
                PersistInheritedRepositorySettings(package, importModel, entered, normalizedPersistence);
                break;
            case RepositorySettingPersistence.SharedMetadata:
            case RepositorySettingPersistence.SharedMetadataSanitisedConnectionString:
            case RepositorySettingPersistence.SharedMetadataWithoutConnectionString:
                RepositoryUserLocalStore.Delete(importModel.PackageFileName!);
                PersistSharedRepositoryMetadata(package, importModel, normalizedPersistence);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(importModel.SettingPersistence), importModel.SettingPersistence, "Unsupported repository setting persistence.");
        }

        package.Save();
    }

    public static DatabaseImportSettingsResolutionData ResolveDatabaseImportSettings(string packageFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageFileName);

        var package = LoadPackage(packageFileName);
        var localSettings = DatabaseUserLocalStore.Load(packageFileName);



        var resolvedSettings = new DatabaseImportResolvedSettings
        {
            EntityNameConvention = GetLayeredValue(localSettings?.EntityNameConvention, package.GetMetadataValue("rdbms-import:entityNameConvention"), "SingularEntity"),
            AttributeNameConvention = GetLayeredValue(localSettings?.AttributeNameConvention, package.GetMetadataValue("rdbms-import:attributeNameConvention"), "Default"),
            TableStereotypes = GetLayeredValue(localSettings?.TableStereotypes, package.GetMetadataValue("rdbms-import:tableStereotypes"), "WhenDifferent"),
            ImportFilterFilePath = GetLayeredValue(localSettings?.ImportFilterFilePath, package.GetMetadataValue("rdbms-import:importFilterFilePath"), "db-import-filter.json"),
            ConnectionString = GetLayeredValue(localSettings?.ConnectionString, package.GetMetadataValue("rdbms-import:connectionString"), null),
            StoredProcedureType = GetLayeredValue(localSettings?.StoredProcedureType, package.GetMetadataValue("rdbms-import:storedProcedureType"), string.Empty),
            SettingPersistence = ResolvePersistedDatabaseSettingPersistence(localSettings, package),
            DatabaseType = GetLayeredValue(localSettings?.DatabaseType, package.GetMetadataValue("rdbms-import:databaseType"), "SqlServer"),
            FilterType = GetLayeredValue(localSettings?.FilterType, package.GetMetadataValue("rdbms-import:filterType"), "include"),
            AllowDeletions = GetLayeredValue(localSettings?.AllowDeletions, package.GetMetadataValue("rdbms-import:allowDeletions"), "true"),
            PreserveAttributeTypes = GetLayeredValue(localSettings?.PreserveAttributeTypes, package.GetMetadataValue("rdbms-import:preserveAttributeTypes"), "true"),
            TypesToExport = GetTypesToExport(GetLayeredValue(localSettings?.TypesToExport, package.GetMetadataValue("rdbms-import:typesToExport"), string.Empty))
        };

        return new DatabaseImportSettingsResolutionData
        {
            Settings = resolvedSettings,
            Source = localSettings != null
            ? DatabaseImportSettingsSource.UserLocal
            : HasSharedMetadata(package)
            ? DatabaseImportSettingsSource.SharedMetadata
            : DatabaseImportSettingsSource.BuiltInDefaults
        };

    }

    public static RepositoryImportSettingsResolutionData ResolveRepositoryImportSettings(string packageFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageFileName);

        var package = LoadPackage(packageFileName);
        var localSettings = RepositoryUserLocalStore.Load(packageFileName);

        var resolvedSettings = new RepositoryImportResolvedSettings
        {
            ConnectionString = GetLayeredValue(localSettings?.ConnectionString, package.GetMetadataValue("rdbms-import-repository:connectionString"), null),
            StoredProcedureType = GetLayeredValue(localSettings?.StoredProcedureType, package.GetMetadataValue("rdbms-import-repository:storedProcedureType"), string.Empty),
            DatabaseType = GetLayeredValue(localSettings?.DatabaseType, package.GetMetadataValue("rdbms-import-repository:databaseType"), string.Empty),
            SettingPersistence = ResolvePersistedRepositorySettingPersistence(localSettings, package)
        };

        return new RepositoryImportSettingsResolutionData
        {
            Settings = resolvedSettings,
            Source = localSettings != null
            ? DatabaseImportSettingsSource.UserLocal
            : HasRepositoryMetadata(package)
            ? DatabaseImportSettingsSource.SharedMetadata
            : DatabaseImportSettingsSource.BuiltInDefaults
        };
    }

    public static string GetUserLocalSettingsPath(string packageFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageFileName);

        return DatabaseUserLocalStore.GetDisplayPath(packageFileName);
    }

    public static string GetRepositoryUserLocalSettingsPath(string packageFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageFileName);

        return RepositoryUserLocalStore.GetDisplayPath(packageFileName);
    }

    public static void HydrateDbSettings(RepositoryImportModel importModel)
    {
        var resolution = ResolveDatabaseImportSettings(importModel.PackageFileName!);
        ApplyInheritedDbSettings(importModel, resolution.Settings);
    }

    /// <summary>
    /// Fills in only what was left blank on the import dialog. An explicitly entered connection string
    /// or database type always wins over the inherited package-level Database Import settings -
    /// otherwise entering one while "Inherit Database Settings" is selected would silently have no
    /// effect. Kept in step with the dialog's own resolution in strategy-stored-procedures-import.ts.
    /// </summary>
    internal static void ApplyInheritedDbSettings(RepositoryImportModel importModel, DatabaseImportResolvedSettings settings)
    {
        if (string.IsNullOrWhiteSpace(importModel.StoredProcedureType))
        {
            importModel.StoredProcedureType = settings.StoredProcedureType;
        }

        if (string.IsNullOrWhiteSpace(importModel.ConnectionString))
        {
            importModel.ConnectionString = settings.ConnectionString!;
        }

        importModel.DatabaseType ??= Enum.TryParse<DatabaseType>(settings.DatabaseType, out var databaseType)
            ? databaseType
            : null;
    }

    private static void PersistSharedDatabaseMetadata(PackageModelPersistable package, DatabaseImportModel importModel)
    {
        var normalizedPersistence = NormalizeDatabaseSettingPersistence(importModel.SettingPersistence);
        package.AddMetadata("rdbms-import:entityNameConvention", importModel.EntityNameConvention);
        package.AddMetadata("rdbms-import:attributeNameConvention", importModel.AttributeNameConvention);
        package.AddMetadata("rdbms-import:tableStereotypes", importModel.TableStereotype);
        package.AddMetadata("rdbms-import:typesToExport", importModel.TypesToExport.Any() ? string.Join(";", importModel.TypesToExport.Select(t => t.ToString())) : string.Empty);
        package.AddMetadata("rdbms-import:importFilterFilePath", importModel.ImportFilterFilePath);
        package.AddMetadata("rdbms-import:storedProcedureType", importModel.StoredProcedureType);
        package.AddMetadata("rdbms-import:filterType", importModel.FilterType);
        package.AddMetadata("rdbms-import:allowDeletions", importModel.AllowDeletions.ToString().ToLowerInvariant());
        package.AddMetadata("rdbms-import:preserveAttributeTypes", importModel.PreserveAttributeTypes.ToString().ToLowerInvariant());
        ProcessSharedConnectionStringSetting(package, importModel, normalizedPersistence);
        package.AddMetadata("rdbms-import:settingPersistence", normalizedPersistence.ToString());
        package.AddMetadata("rdbms-import:databaseType", importModel.DatabaseType.ToString());
    }

    private static void PersistSharedRepositoryMetadata(PackageModelPersistable package, RepositoryImportModel importModel, RepositorySettingPersistence normalizedPersistence)
    {
        package.AddMetadata("rdbms-import-repository:storedProcedureType", importModel.StoredProcedureType);
        ProcessRepositoryConnectionStringSetting(package, importModel, normalizedPersistence);
        package.AddMetadata("rdbms-import-repository:settingPersistence", normalizedPersistence.ToString());
        ProcessRepositoryDatabaseTypeSetting(package, importModel);
    }

    /// <summary>
    /// "Inherit Database Settings" still has to remember whatever the user typed over the top of what
    /// it inherits - otherwise an entered connection string takes effect for one run and is then
    /// silently lost. Only genuinely entered values are stored, so an import that inherited everything
    /// leaves no repository-level override behind and keeps following the Database Import settings.
    /// User-local is deliberate: the team-shared decision was already made at the Database Import
    /// level, so a personal override must never leak into shared package metadata.
    /// </summary>
    private static void PersistInheritedRepositorySettings(
        PackageModelPersistable package,
        RepositoryImportModel importModel,
        RepositoryEnteredSettings entered,
        RepositorySettingPersistence normalizedPersistence)
    {
        if (entered.HasConnectionOverride)
        {
            RepositoryUserLocalStore.Save(importModel.PackageFileName!, CreateRepositoryUserLocalSettings(entered, normalizedPersistence));
            RemoveRepositoryMetadata(package);
            return;
        }

        // Nothing was overridden, so clearing the override is how the user gets back to pure inheritance.
        RepositoryUserLocalStore.Delete(importModel.PackageFileName!);
        package.AddMetadata("rdbms-import-repository:storedProcedureType", importModel.StoredProcedureType);
        package.AddMetadata("rdbms-import-repository:settingPersistence", normalizedPersistence.ToString());
        package.RemoveMetadata("rdbms-import-repository:connectionString");
        package.RemoveMetadata("rdbms-import-repository:databaseType");
    }

    private static RepositoryImportLocalSettings CreateRepositoryUserLocalSettings(RepositoryImportModel importModel, RepositorySettingPersistence persistence)
    {
        return new RepositoryImportLocalSettings
        {
            ConnectionString = importModel.ConnectionString,
            DatabaseType = importModel.DatabaseType?.ToString(),
            StoredProcedureType = importModel.StoredProcedureType,
            SettingPersistence = persistence.ToString()
        };
    }

    private static RepositoryImportLocalSettings CreateRepositoryUserLocalSettings(RepositoryEnteredSettings entered, RepositorySettingPersistence persistence)
    {
        return new RepositoryImportLocalSettings
        {
            ConnectionString = entered.ConnectionString,
            DatabaseType = entered.DatabaseType?.ToString(),
            StoredProcedureType = entered.StoredProcedureType,
            SettingPersistence = persistence.ToString()
        };
    }

    private static void RemoveRepositoryMetadata(PackageModelPersistable package)
    {
        package.RemoveMetadata("rdbms-import-repository:storedProcedureType");
        package.RemoveMetadata("rdbms-import-repository:connectionString");
        package.RemoveMetadata("rdbms-import-repository:settingPersistence");
        package.RemoveMetadata("rdbms-import-repository:databaseType");
    }

    private static DatabaseImportLocalSettings CreateUserLocalSettings(DatabaseImportModel importModel)
    {
        return new DatabaseImportLocalSettings
        {
            EntityNameConvention = importModel.EntityNameConvention,
            AttributeNameConvention = importModel.AttributeNameConvention,
            TableStereotypes = importModel.TableStereotype,
            TypesToExport = importModel.TypesToExport.Any() ? string.Join(";", importModel.TypesToExport.Select(t => t.ToString())) : string.Empty,
            ImportFilterFilePath = importModel.ImportFilterFilePath,
            ConnectionString = importModel.ConnectionString,
            StoredProcedureType = importModel.StoredProcedureType,
            SettingPersistence = DatabaseSettingPersistence.UserLocal.ToString(),
            DatabaseType = importModel.DatabaseType.ToString(),
            FilterType = importModel.FilterType,
            AllowDeletions = importModel.AllowDeletions.ToString().ToLowerInvariant(),
            PreserveAttributeTypes = importModel.PreserveAttributeTypes.ToString().ToLowerInvariant()
        };
    }

    private static void RemoveDatabaseMetadata(PackageModelPersistable package)
    {
        package.RemoveMetadata("rdbms-import:entityNameConvention");
        package.RemoveMetadata("rdbms-import:attributeNameConvention");
        package.RemoveMetadata("rdbms-import:tableStereotypes");
        package.RemoveMetadata("rdbms-import:typesToExport");
        package.RemoveMetadata("rdbms-import:importFilterFilePath");
        package.RemoveMetadata("rdbms-import:storedProcedureType");
        package.RemoveMetadata("rdbms-import:filterType");
        package.RemoveMetadata("rdbms-import:allowDeletions");
        package.RemoveMetadata("rdbms-import:preserveAttributeTypes");
        package.RemoveMetadata("rdbms-import:connectionString");
        package.RemoveMetadata("rdbms-import:settingPersistence");
        package.RemoveMetadata("rdbms-import:databaseType");
    }

    private static readonly string[] PasswordKeywords = ["Password", "PWD"];

    /// <summary>
    /// Strips the password from a connection string for storage in team-shared metadata. Uses the
    /// provider-agnostic <see cref="DbConnectionStringBuilder"/>: the module process cannot load
    /// Microsoft.Data.SqlClient (only its throwing facade ships alongside the module), and a
    /// SQL-Server-specific builder rejects PostgreSQL keywords outright.
    /// </summary>
    internal static string SanitiseConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString ?? string.Empty;
        }

        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

        // No synonym expansion here, unlike SqlConnectionStringBuilder - so remove every spelling.
        var removed = false;
        foreach (var keyword in PasswordKeywords)
        {
            removed |= builder.Remove(keyword);
        }

        // Placeholder so whoever picks up the shared metadata can see where to put their own password.
        return removed ? "Password=  ;" + builder.ConnectionString : builder.ConnectionString;
    }

    private static void ProcessSharedConnectionStringSetting(PackageModelPersistable package, DatabaseImportModel settings, DatabaseSettingPersistence normalizedPersistence)
    {
        var connectionString = settings.ConnectionString;

        if (normalizedPersistence == DatabaseSettingPersistence.SharedMetadataSanitisedConnectionString)
        {
            connectionString = SanitiseConnectionString(connectionString);
        }

        if (normalizedPersistence == DatabaseSettingPersistence.SharedMetadataWithoutConnectionString)
        {
            package.RemoveMetadata("rdbms-import:connectionString");
        }
        else
        {
            package.AddMetadata("rdbms-import:connectionString", connectionString);
        }
    }

    private static void ProcessRepositoryDatabaseTypeSetting(PackageModelPersistable package, RepositoryImportModel settings)
    {
        package.AddMetadata("rdbms-import-repository:databaseType", settings.DatabaseType.ToString());
    }

    private static void ProcessRepositoryConnectionStringSetting(PackageModelPersistable package, RepositoryImportModel settings, RepositorySettingPersistence normalizedPersistence)
    {
        var connectionString = settings.ConnectionString;

        if (normalizedPersistence == RepositorySettingPersistence.SharedMetadataSanitisedConnectionString)
        {
            connectionString = SanitiseConnectionString(connectionString);
        }

        if (normalizedPersistence == RepositorySettingPersistence.SharedMetadataWithoutConnectionString)
        {
            package.RemoveMetadata("rdbms-import-repository:connectionString");
        }
        else
        {
            package.AddMetadata("rdbms-import-repository:connectionString", connectionString);
        }
    }

    private static DatabaseSettingPersistence ResolvePersistedDatabaseSettingPersistence(DatabaseImportLocalSettings? localSettings, PackageModelPersistable package)
    {
        if (localSettings != null)
        {
            return DatabaseSettingPersistence.UserLocal;
        }

        var metadataPersistence = package.GetMetadataValue("rdbms-import:settingPersistence");
        if (!string.IsNullOrWhiteSpace(metadataPersistence))
        {
            return NormalizeDatabaseSettingPersistence(Enum.Parse<DatabaseSettingPersistence>(metadataPersistence));
        }

        if (HasLegacyDatabaseMetadata(package))
        {
            return DatabaseSettingPersistence.SharedMetadata;
        }

        return DatabaseSettingPersistence.UserLocal;
    }


    private static DatabaseSettingPersistence NormalizeDatabaseSettingPersistence(DatabaseSettingPersistence persistence)
    {
        return persistence switch
        {
            DatabaseSettingPersistence.All => DatabaseSettingPersistence.SharedMetadata,
            DatabaseSettingPersistence.AllSanitisedConnectionString => DatabaseSettingPersistence.SharedMetadataSanitisedConnectionString,
            DatabaseSettingPersistence.AllWithoutConnectionString => DatabaseSettingPersistence.SharedMetadataWithoutConnectionString,
            _ => persistence
        };
    }

    private static RepositorySettingPersistence ResolvePersistedRepositorySettingPersistence(RepositoryImportLocalSettings? localSettings, PackageModelPersistable package)
    {
        if (localSettings != null)
        {
            return ResolveLocalRepositorySettingPersistence(localSettings);
        }

        var metadataPersistence = package.GetMetadataValue("rdbms-import-repository:settingPersistence");
        if (!string.IsNullOrWhiteSpace(metadataPersistence))
        {
            return NormalizeRepositorySettingPersistence(Enum.Parse<RepositorySettingPersistence>(metadataPersistence));
        }

        if (HasLegacyRepositoryMetadata(package))
        {
            return RepositorySettingPersistence.SharedMetadata;
        }

        return RepositorySettingPersistence.None;
    }

    /// <summary>
    /// Which persistence choice a user-local settings file represents. An InheritDb override also
    /// lives in user-local storage, so the stored value is honoured rather than assumed to be
    /// UserLocal - otherwise the dialog would come back showing "Only for me" for someone who chose
    /// "Inherit Database Settings". Files written before overrides were supported record "UserLocal"
    /// and still resolve to it, as does anything unrecognised.
    /// </summary>
    internal static RepositorySettingPersistence ResolveLocalRepositorySettingPersistence(RepositoryImportLocalSettings localSettings)
    {
        ArgumentNullException.ThrowIfNull(localSettings);

        return Enum.TryParse<RepositorySettingPersistence>(localSettings.SettingPersistence, out var localPersistence)
            ? NormalizeRepositorySettingPersistence(localPersistence)
            : RepositorySettingPersistence.UserLocal;
    }

    internal static RepositorySettingPersistence NormalizeRepositorySettingPersistence(RepositorySettingPersistence persistence)
    {
        return persistence switch
        {
            RepositorySettingPersistence.All => RepositorySettingPersistence.SharedMetadata,
            RepositorySettingPersistence.AllSanitisedConnectionString => RepositorySettingPersistence.SharedMetadataSanitisedConnectionString,
            RepositorySettingPersistence.AllWithoutConnectionString => RepositorySettingPersistence.SharedMetadataWithoutConnectionString,
            _ => persistence
        };
    }

    private static string[] GetTypesToExport(string persistedValue)
    {
        if (string.IsNullOrWhiteSpace(persistedValue))
        {
            return [];
        }

        return persistedValue.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string GetLayeredValue(string? localValue, string? metadataValue, string? defaultValue)
    {
        if (!string.IsNullOrWhiteSpace(localValue))
        {
            return localValue;
        }

        if (!string.IsNullOrWhiteSpace(metadataValue))
        {
            return metadataValue;
        }

        return defaultValue ?? string.Empty;
    }

    private static bool HasSharedMetadata(PackageModelPersistable package)
    {
        return !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:settingPersistence")) ||
            HasLegacyDatabaseMetadata(package);
    }

    private static bool HasRepositoryMetadata(PackageModelPersistable package)
    {
        return !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import-repository:settingPersistence")) ||
            HasLegacyRepositoryMetadata(package);
    }

    private static bool HasLegacyRepositoryMetadata(PackageModelPersistable package)
    {
        return !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import-repository:storedProcedureType")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import-repository:connectionString")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import-repository:databaseType"));
    }

    private static bool HasLegacyDatabaseMetadata(PackageModelPersistable package)
    {
        return !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:entityNameConvention")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:attributeNameConvention")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:tableStereotypes")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:typesToExport")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:importFilterFilePath")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:storedProcedureType")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:filterType")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:allowDeletions")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:preserveAttributeTypes")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:connectionString")) ||
            !string.IsNullOrWhiteSpace(package.GetMetadataValue("rdbms-import:databaseType"));
    }

    // We can't use PackageModelPersistable.Load since it uses the underlying cached versions

    // Also it should ONLY load the package as to prevent unfortunate model corruption
    private static PackageModelPersistable LoadPackage(string packagePath)
    {
        var package = XmlSerializationHelper.LoadFromFile<PackageModelPersistable>(packagePath, loadThisFileOnly: true, skipCache: true);
        foreach (var reference in package.References.Where(reference => !string.IsNullOrWhiteSpace(reference.RelativePath)))
        {
            reference.AbsolutePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(package.DirectoryPath, reference.RelativePath));
        }

        return package;
    }
}

