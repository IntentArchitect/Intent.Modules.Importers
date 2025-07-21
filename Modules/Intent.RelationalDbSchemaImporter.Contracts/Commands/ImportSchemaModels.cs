using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.Contracts.Commands;

public class ImportSchemaRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public string? ImportFilterFilePath { get; set; }
    public HashSet<ExportType> TypesToExport { get; set; } = [ExportType.Table, ExportType.View, ExportType.StoredProcedure, ExportType.Index];
    public List<string> StoredProcNames { get; set; } = [];
    public DatabaseType DatabaseType { get; set; } = DatabaseType.Unspecified;
}

public class ImportSchemaResult
{
    public DatabaseSchema SchemaData { get; set; } = new();
}
