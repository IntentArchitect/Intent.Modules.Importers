namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

public class DatabaseSchema
{
    public string DatabaseName { get; set; } = string.Empty;
    public List<TableSchema> Tables { get; set; } = [];
    public List<ViewSchema> Views { get; set; } = [];
    /// <summary>
    /// Legacy stored procedures list - will be migrated to DatabaseRoutines
    /// </summary>
    public List<StoredProcedureSchema> StoredProcedures { get; set; } = [];
    /// <summary>
    /// Unified database routines (stored procedures, functions, etc.)
    /// </summary>
    public List<DatabaseRoutineSchema> DatabaseRoutines { get; set; } = [];
    public List<UserDefinedTableTypeSchema> UserDefinedTableTypes { get; set; } = [];
}
