namespace Intent.Modules.Rdbms.Importer.Tasks.Models;

public class DatabaseImportSettingsResolutionInput
{
    public string ApplicationId { get; set; } = null!;
    public string PackageId { get; set; } = null!;
}

public class DatabaseImportSettingsResolutionResult
{
    public string EntityNameConvention { get; set; } = null!;
    public string AttributeNameConvention { get; set; } = null!;
    public string TableStereotypes { get; set; } = null!;
    public string TypesToExport { get; set; } = null!;
    public string? ConnectionString { get; set; }
    public string ImportFilterFilePath { get; set; } = null!;
    public string StoredProcedureType { get; set; } = null!;
    public string SettingPersistence { get; set; } = null!;
    public string DatabaseType { get; set; } = null!;
    public string FilterType { get; set; } = null!;
    public string AllowDeletions { get; set; } = null!;
    public string PreserveAttributeTypes { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string? UserLocalSettingsPath { get; set; }
}


internal class DatabaseImportSettingsResolutionData
{

    public DatabaseImportResolvedSettings Settings { get; set; } = new();
    public DatabaseImportSettingsSource Source { get; set; }
}

internal enum DatabaseImportSettingsSource
{
    BuiltInDefaults,
    SharedMetadata,
    UserLocal
}

internal class DatabaseImportResolvedSettings
{
    public string EntityNameConvention { get; set; } = null!;
    public string AttributeNameConvention { get; set; } = null!;
    public string TableStereotypes { get; set; } = null!;
    public string[] TypesToExport { get; set; } = [];
    public string ImportFilterFilePath { get; set; } = null!;
    public string? ConnectionString { get; set; }
    public string StoredProcedureType { get; set; } = null!;
    public DatabaseSettingPersistence SettingPersistence { get; set; }
    public string DatabaseType { get; set; } = null!;
    public string FilterType { get; set; } = null!;
    public string AllowDeletions { get; set; } = null!;
    public string PreserveAttributeTypes { get; set; } = null!;
}

internal class DatabaseImportLocalSettings
{
    public string? EntityNameConvention { get; set; }
    public string? AttributeNameConvention { get; set; }
    public string? TableStereotypes { get; set; }
    public string? TypesToExport { get; set; }
    public string? ImportFilterFilePath { get; set; }
    public string? ConnectionString { get; set; }
    public string? StoredProcedureType { get; set; }
    public string? SettingPersistence { get; set; }
    public string? DatabaseType { get; set; }
    public string? FilterType { get; set; }
    public string? AllowDeletions { get; set; }
    public string? PreserveAttributeTypes { get; set; }
}

public class RepositoryImportSettingsResolutionResult
{
    public string? ConnectionString { get; set; }
    public string DatabaseType { get; set; } = null!;
    public string StoredProcedureType { get; set; } = null!;
    public string SettingPersistence { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string? UserLocalSettingsPath { get; set; }

    /// <summary>
    /// The resolved <em>database</em> import values, used by the dialog's
    /// "Inherit Database Settings" mode.
    /// </summary>
    public string? InheritedConnectionString { get; set; }

    public string InheritedDatabaseType { get; set; } = null!;
}

internal class RepositoryImportSettingsResolutionData
{
    public RepositoryImportResolvedSettings Settings { get; set; } = new();
    public DatabaseImportSettingsSource Source { get; set; }
}

internal class RepositoryImportResolvedSettings
{
    public string? ConnectionString { get; set; }
    public string DatabaseType { get; set; } = null!;
    public string StoredProcedureType { get; set; } = null!;
    public RepositorySettingPersistence SettingPersistence { get; set; }
}

/// <remarks>
/// Deliberately all-nullable-strings so that a partially written settings file degrades to
/// defaults rather than throwing.
/// </remarks>
internal class RepositoryImportLocalSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseType { get; set; }
    public string? StoredProcedureType { get; set; }
    public string? SettingPersistence { get; set; }
}

