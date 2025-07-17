using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Contracts.Schema;

namespace Intent.RelationalDbSchemaImporter.Contracts.Models;

public class ImportSchemaRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public string PackageFileName { get; set; } = string.Empty;
    public string? ImportFilterFilePath { get; set; }
    public string? ApplicationId { get; set; }
    public EntityNameConvention EntityNameConvention { get; set; } = EntityNameConvention.SingularEntity;
    public TableStereotype TableStereotype { get; set; } = TableStereotype.WhenDifferent;
    public HashSet<ExportType> TypesToExport { get; set; } = [ExportType.Table, ExportType.View, ExportType.StoredProcedure, ExportType.Index];
    public StoredProcedureType StoredProcedureType { get; set; } = StoredProcedureType.Default;
    public string? RepositoryElementId { get; set; }
    public List<string> StoredProcNames { get; set; } = [];
}

public class ImportSchemaResult
{
    public DatabaseSchema SchemaData { get; set; } = new();
}
