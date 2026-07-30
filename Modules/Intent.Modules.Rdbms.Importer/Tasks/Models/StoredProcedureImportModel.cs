using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Models;

public class RepositoryImportModel
{
    public string ApplicationId { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    public string EntityNameConvention { get; set; } = null!;
    public string TableStereotype { get; set; } = null!;
    public string? StoredProcedureType { get; set; }
    public string? RepositoryElementId { get; set; }
    public string? PackageFileName { get; set; }
    public RepositorySettingPersistence SettingPersistence { get; set; } = RepositorySettingPersistence.None;
    
    // BEGIN - ImportSchemaRequest
    public string ConnectionString { get; set; } = null!;
    public List<string> TypesToExport { get; set; } = [];
    public List<string> StoredProcNames { get; set; } = [];
    public DatabaseType? DatabaseType { get; set; }
// END - ImportSchemaRequest
}

/// <summary>
/// What the user actually typed on the stored procedure import dialog, snapshotted before
/// SettingsHelper.ApplyInheritedDbSettings fills in the blanks from the inherited Database Import
/// settings. Persistence works off this rather than the hydrated model:
/// otherwise "Inherit Database Settings" would write the inherited connection string back out as a
/// permanent per-repository override, even when nothing was entered.
/// </summary>
internal sealed record RepositoryEnteredSettings(
    string? ConnectionString,
    DatabaseType? DatabaseType,
    string? StoredProcedureType)
{
    public static RepositoryEnteredSettings From(RepositoryImportModel importModel) =>
        new(importModel.ConnectionString, importModel.DatabaseType, importModel.StoredProcedureType);

    /// <summary>
    /// Whether the user supplied connection settings of their own. Only then does an inheriting
    /// import have anything worth remembering at the repository level.
    /// </summary>
    public bool HasConnectionOverride =>
        !string.IsNullOrWhiteSpace(ConnectionString) || DatabaseType is not null;
}

public enum RepositorySettingPersistence
{
    None,
    InheritDb,
    UserLocal,
    SharedMetadataSanitisedConnectionString,
    SharedMetadataWithoutConnectionString,
    SharedMetadata,

    // Legacy members, retained so existing package metadata still parses. Normalized to their
    // SharedMetadata* equivalents by SettingsHelper.NormalizeRepositorySettingPersistence.
    All,
    AllSanitisedConnectionString,
    AllWithoutConnectionString
}
