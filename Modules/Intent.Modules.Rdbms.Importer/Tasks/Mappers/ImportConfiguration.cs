using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

internal class ImportConfiguration
{
    public string? ApplicationId { get; set; }
    public EntityNameConvention EntityNameConvention { get; set; } = EntityNameConvention.SingularEntity;
    public AttributeNameConvention AttributeNameConvention { get; set; } = AttributeNameConvention.Default;
    public TableStereotype TableStereotype { get; set; } = TableStereotype.WhenDifferent;
    public StoredProcedureType StoredProcedureType { get; set; } = StoredProcedureType.Default;
    public string? RepositoryElementId { get; set; }
    public string? PackageFileName { get; set; }

    // BEGIN - ImportSchemaRequest
    public string? ConnectionString { get; set; }
    public string? ImportFilterFilePath { get; set; }
    public HashSet<ExportType> TypesToExport { get; set; } = [ExportType.Table, ExportType.View, ExportType.StoredProcedure, ExportType.Index];
    public List<string> StoredProcNames { get; set; } = [];
    public DatabaseType DatabaseType { get; set; }
    // END - ImportSchemaRequest
    
    /// <summary>
    /// Indicates whether to remove attributes/elements that no longer exist in the database.
    /// Only elements with external references (imported from DB) will be removed.
    /// </summary>
    public bool AllowDeletions { get; set; } = true;
    
    /// <summary>
    /// Indicates whether to preserve existing attribute type references during import.
    /// When true (default), custom type mappings (e.g., enums) set by the user will not be overridden.
    /// When false, attribute types will be updated to match the database column types.
    /// </summary>
    public bool PreserveAttributeTypes { get; set; } = true;
    
    public bool ExportTables()
    {
        return TypesToExport.Contains(ExportType.Table);
    }

    public bool ExportViews()
    {
        return TypesToExport.Contains(ExportType.View);
    }

    public bool ExportIndexes()
    {
        return TypesToExport.Contains(ExportType.Index);
    }
    
    public bool ExportStoredProcedures()
    {
        return TypesToExport.Contains(ExportType.StoredProcedure);
    }
}
