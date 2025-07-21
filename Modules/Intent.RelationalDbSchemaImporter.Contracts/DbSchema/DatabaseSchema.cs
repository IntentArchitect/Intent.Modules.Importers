namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

public class DatabaseSchema
{
    public string DatabaseName { get; set; } = string.Empty;
    public List<TableSchema> Tables { get; set; } = [];
    public List<ViewSchema> Views { get; set; } = [];
    public List<StoredProcedureSchema> StoredProcedures { get; set; } = [];
    public List<UserDefinedTableTypeSchema> UserDefinedTableTypes { get; set; } = [];
}
