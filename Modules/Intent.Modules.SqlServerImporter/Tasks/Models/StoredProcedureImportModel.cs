using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

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
    public DatabaseType DatabaseType { get; set; }
    // END - ImportSchemaRequest
}

public enum RepositorySettingPersistence
{
    None,
    InheritDb,
    AllSanitisedConnectionString,
    AllWithoutConnectionString,
    All
}