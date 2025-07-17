using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class ImportConfiguration
{
    public string? ApplicationId { get; set; }
    public EntityNameConvention EntityNameConvention { get; set; } = EntityNameConvention.SingularEntity;
    public TableStereotype TableStereotype { get; set; } = TableStereotype.WhenDifferent;
    public HashSet<ExportType> TypesToExport { get; set; } = [ExportType.Table, ExportType.View, ExportType.StoredProcedure, ExportType.Index];
    public string? ImportFilterFilePath { get; set; }
    public StoredProcedureType StoredProcedureType { get; set; } = StoredProcedureType.Default;
    public string? RepositoryElementId { get; set; }
    public List<string> StoredProcNames { get; set; } = [];
    public string? ConnectionString { get; set; }
    public string? PackageFileName { get; set; }

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
